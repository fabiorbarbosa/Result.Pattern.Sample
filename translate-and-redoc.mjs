import fs from "fs";
import { globSync } from "glob";
import YAML from "yamljs";
import translate from "google-translate-api-x";
import { execSync } from "child_process";

const SEP = "|||<SEP>|||";
const CHUNK_SIZE = 100; // Ajuste aqui para outros tamanhos de lote

// Busca todos os YAML/YML (exceto os já traduzidos)
const files = globSync("*.yml")
  .concat(globSync("*.yaml"))
  .filter((f) => !f.endsWith(".en.yaml") && !f.endsWith(".en.yml"));

// Função para dividir array em lotes (chunks)
function chunkArray(arr, chunkSize) {
  const chunks = [];
  for (let i = 0; i < arr.length; i += chunkSize) {
    chunks.push(arr.slice(i, i + chunkSize));
  }
  return chunks;
}

// Funções auxiliares
function extractStrings(obj, arr = [], path = []) {
  for (const key in obj) {
    if (typeof obj[key] === "string") {
      arr.push({ path: [...path, key], value: obj[key] });
    } else if (typeof obj[key] === "object" && obj[key] !== null) {
      extractStrings(obj[key], arr, [...path, key]);
    }
  }
  return arr;
}
function setByPath(obj, path, value) {
  const last = path.pop();
  let temp = obj;
  for (const p of path) temp = temp[p];
  temp[last] = value;
}

// Traduz e salva
for (const file of files) {
  const doc = YAML.load(file);
  const stringsToTranslate = extractStrings(doc);
  if (stringsToTranslate.length === 0) continue;

  // Paginando a tradução em lotes
  const chunks = chunkArray(stringsToTranslate, CHUNK_SIZE);
  let translatedArray = [];
  for (const chunk of chunks) {
    const batchText = chunk.map((item) => item.value).join(SEP);
    console.log(`Traduzindo ${chunk.length} textos do arquivo ${file}...`);
    let translatedText = null;
    try {
      translatedText = (await translate(batchText, { to: "en" })).text;
    } catch (e) {
      console.error("Erro ao traduzir chunk:", e.message);
      // Em caso de erro, coloca os originais no lugar
      translatedText = chunk.map((item) => item.value).join(SEP);
    }
    if (typeof translatedText !== "string") {
      console.error("Tradução retornou nulo! Usando texto original.");
      translatedArray = translatedArray.concat(chunk.map((item) => item.value));
    } else {
      translatedArray = translatedArray.concat(translatedText.split(SEP));
    }
  }

  stringsToTranslate.forEach((item, i) =>
    setByPath(doc, [...item.path], translatedArray[i])
  );

  // Salva YAML traduzido
  const outName = file.replace(/\.ya?ml$/, ".en.yaml");
  fs.writeFileSync(outName, YAML.stringify(doc, 10, 2), "utf8");
  console.log(`Arquivo traduzido: ${outName}`);
}

// Gera HTML com o Redocly CLI
const allYamlFiles = globSync("*.yml").concat(globSync("*.yaml"));
for (const yaml of allYamlFiles) {
  const htmlName = yaml.replace(/\.ya?ml$/, ".html");
  try {
    console.log(`Gerando HTML: ${htmlName}`);
    execSync(`npx redocly build-docs "${yaml}" -o "${htmlName}"`);
  } catch (e) {
    console.error(`Erro ao gerar o HTML para ${yaml}:`, e.message);
  }
}
console.log("Processo finalizado!");
