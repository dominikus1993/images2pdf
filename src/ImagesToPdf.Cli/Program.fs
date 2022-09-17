// For more information see https://aka.ms/fsharp-console-apps

open System
open System.IO
open QuestPDF
open QuestPDF.Helpers
open QuestPDF.Infrastructure
open QuestPDF.Fluent
open SixLabors.ImageSharp;
open SixLabors.ImageSharp.Processing;

let resize (size: QuestPDF.Infrastructure.Size) (ctx: IImageProcessingContext) : unit =
    ctx.Resize(size.Width |> int, size.Height |> int, KnownResamplers.Lanczos3)
    ()

let loadImage (path) (size: QuestPDF.Infrastructure.Size) : byte array =
    use image = Image.Load(path = path)
    image.Mutate(resize(size))
    Array.empty<byte>
    

let loadImages (path:string) (page: PageDescriptor) =
    let files = Directory.GetFiles(path, "*.png")
    for file in files do
        let f = Func<QuestPDF.Infrastructure.Size, byte array>(loadImage(file))
        page.Content().Image(f)
    ()

    

let createPage (path: string) (page: PageDescriptor): unit =
    page.Size(PageSizes.A4)
    page.Margin(2f, Unit.Centimetre);
    page.PageColor(Colors.White);
    loadImages path page
    ()

let createContainer (path:string) (ct:IDocumentContainer):unit = 
    ct.Page(createPage(path))
    ()

Document.Create(createContainer("./"))
