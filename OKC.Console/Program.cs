using OKC.Infrastructure.Services;

var parser = new HtmlQuestionParser();
var repository = new JsonQuestionRepository(Path.GetFullPath("..\\..\\..\\..\\outputs\\4-вся.json"));
var processor = new QuestionProcessor(parser, repository);

var htmlFiles = Directory.GetFiles(Path.GetFullPath("..\\..\\..\\..\\inputs\\4-вся"), "*.html");
processor.ProcessFiles(htmlFiles);