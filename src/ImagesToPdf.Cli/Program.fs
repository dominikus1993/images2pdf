// For more information see https://aka.ms/fsharp-console-apps

open System
open System.IO
open QuestPDF
open QuestPDF.Helpers
open QuestPDF.Infrastructure
open QuestPDF.Fluent
open SixLabors.ImageSharp;
open SixLabors.ImageSharp.Formats
open SixLabors.ImageSharp.Formats.Png
open SixLabors.ImageSharp.Processing;

let resize (size: QuestPDF.Infrastructure.Size) (ctx: IImageProcessingContext) : unit =
    ctx.Resize(size.Width |> int, size.Height |> int, KnownResamplers.Lanczos3)
    ()

let loadImage (path) (size: QuestPDF.Infrastructure.Size) : byte array =
    use image = Image.Load(path = path)
    image.Mutate(resize(size))
    use memory = new MemoryStream()
    image.Save(memory, PngFormat.Instance)
    memory.ToArray()

let createPage (path: string) (page: PageDescriptor): unit =
    page.Size(PageSizes.A4)
    page.Margin(2f, Unit.Centimetre);
    page.PageColor(Colors.White);
    page.Content().Image(loadImage(path))
    ()

let createContainer (dir: string) (ct:IDocumentContainer):unit =
    let files = Directory.GetFiles(dir, "*.jpg")
    for file in files do
        ct.Page(createPage(file))
    ()

let getDir(args: string[]): string =
    if args.Length = 1 then
        args[0]
    else
        System.Environment.CurrentDirectory

let getPdfName (args: string[]): string =
    if args.Length = 2 then
        args[1]
    else
        "test.pdf"

let args = Environment.GetCommandLineArgs()[1..]

let dir = getDir(args)

let document = Document.Create(createContainer(dir))

document.GeneratePdf(getPdfName(args))
