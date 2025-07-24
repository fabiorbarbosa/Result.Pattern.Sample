import fs from "fs";
import { globSync } from "glob";
import YAML from "yamljs";
import translate from "google-translate-api-x";
import { execSync } from "child_process";

const SEP = "|||<SEP>|||";

// 1. Busca todos os YAML/YML (menos os já traduzidos)
const files = globSync("*.yml")
  .concat(globSync("*.yaml"))
  .filter((f) => !f.endsWith(".en.yaml") && !f.endsWith(".en.yml"));

// 2. Funções auxiliares
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

// 3. Traduz e salva
for (const file of files) {
  const doc = YAML.load(file);
  const stringsToTranslate = extractStrings(doc);
  if (stringsToTranslate.length === 0) continue;

  const batchText = stringsToTranslate.map((item) => item.value).join(SEP);
  console.log(`Traduzindo: ${file}`);
  let translatedText;
  try {
    translatedText = (await translate(batchText, { to: "en" })).text;
  } catch (e) {
    console.error(`Erro ao traduzir ${file}:`, e.message);
    continue;
  }
  const translatedArray = translatedText.split(SEP);
  stringsToTranslate.forEach((item, i) =>
    setByPath(doc, [...item.path], translatedArray[i])
  );

  // Salva YAML traduzido
  const outName = file.replace(/\.ya?ml$/, ".en.yaml");
  fs.writeFileSync(outName, YAML.stringify(doc, 10, 2), "utf8");
  console.log(`Arquivo traduzido: ${outName}`);
}

// 4. Gera HTML com o Redocly CLI
const allYamlFiles = globSync("*.yml").concat(globSync("*.yaml"));
for (const yaml of allYamlFiles) {
  const htmlName = yaml.replace(/\.ya?ml$/, ".html");
  try {
    console.log(`Gerando HTML: ${htmlName}`);
    execSync(`redocly build-docs "${yaml}" -o "${htmlName}"`);
  } catch (e) {
    console.error(`Erro ao gerar o HTML para ${yaml}:`, e.message);
  }
}
console.log("Processo finalizado!");
