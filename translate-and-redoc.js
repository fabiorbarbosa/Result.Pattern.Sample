const fs = require("fs");
const path = require("path");
const YAML = require("yamljs");
const translate = require("google-translate-api-x");
const glob = require("glob");
const { execSync } = require("child_process");

const directory = "./yamls"; // altere para seu diretório de YAMLs
const targetLang = "en";

async function translateFields(obj) {
  for (const key in obj) {
    if (typeof obj[key] === "object" && obj[key] !== null) {
      await translateFields(obj[key]);
    } else if (
      (key === "description" || key === "summary") &&
      typeof obj[key] === "string"
    ) {
      try {
        const result = await translate(obj[key], { to: targetLang });
        obj[key] = result.text;
      } catch (err) {
        console.log(`Falha ao traduzir: "${obj[key]}"`, err.message);
      }
    }
  }
}

async function processFile(file) {
  console.log(`Traduzindo: ${file}`);
  const doc = YAML.load(file);
  await translateFields(doc);

  const outName = file.replace(/\.yaml$/, `.en.yaml`);
  YAML.write(outName, doc, 10, 2);
  console.log(`Arquivo traduzido salvo em: ${outName}`);
  return outName;
}

(async () => {
  // 1. Encontre todos os arquivos *.yaml (exceto *.en.yaml)
  const files = glob
    .sync(path.join(directory, "*.yaml"))
    .filter((f) => !f.endsWith(".en.yaml"));

  // 2. Traduzir e salvar arquivos .en.yaml
  const translatedFiles = [];
  for (const file of files) {
    const outName = await processFile(file);
    translatedFiles.push(outName);
  }

  // 3. Execute o Redocly CLI em todos (originais + traduzidos)
  const allYamlFiles = glob.sync(path.join(directory, "*.yaml"));
  for (const yamlFile of allYamlFiles) {
    const htmlFile = yamlFile.replace(/\.yaml$/, ".html");
    console.log(`Gerando Redocly HTML para: ${yamlFile}`);
    try {
      execSync(`npx @redocly/cli build-docs "${yamlFile}" -o "${htmlFile}"`, {
        stdio: "inherit",
      });
    } catch (err) {
      console.error(`Erro ao gerar HTML para ${yamlFile}:`, err.message);
    }
  }
  console.log("Processo finalizado!");
})();
