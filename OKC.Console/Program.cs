using OKC.Infrastructure.Services;

var parser = new HtmlQuestionParser();
var repository = new JsonQuestionRepository(Path.GetFullPath("..\\..\\..\\..\\outputs\\output.json"));
var processor = new QuestionProcessor(parser, repository);

var htmlFiles = Directory.GetFiles("C:\\Users\\Admin\\Desktop\\4 -До 1000", "*.html");
processor.ProcessFiles(htmlFiles);