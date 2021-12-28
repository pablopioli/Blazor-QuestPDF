using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BlazorQuestPDF
{
    public class InvoiceDocument : IDocument
    {
        public InvoiceModel Model { get; }

        public InvoiceDocument(InvoiceModel model)
        {
            Model = model;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container
                .Page(page =>
                {
                    page.Margin(50);

                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                });
        }

        private void ComposeHeader(IContainer container)
        {
            var titleTextStyle = TextStyle.Default.Size(20).SemiBold().Color(Colors.Blue.Medium);

            container.Row(row =>
            {
                row.RelativeColumn().Stack(stack =>
                              {
                                  stack.Item().Text($"Invoice #{Model.InvoiceNumber}", titleTextStyle);

                                  stack.Item().Text(text =>
                                  {
                                      text.Span("Issue date: ", TextStyle.Default.SemiBold());
                                      text.Span($"{Model.IssueDate:d}");
                                  });

                                  stack.Item().Text(text =>
                                  {
                                      text.Span("Due date: ", TextStyle.Default.SemiBold());
                                      text.Span($"{Model.DueDate:d}");
                                  });
                              });

                row.ConstantColumn(100).Height(50).Placeholder();
            });
        }

        private void ComposeContent(IContainer container)
        {
            container.PaddingVertical(40).Stack(column =>
            {
                column.Spacing(20);

                column.Item().Row(row =>
                {
                    row.RelativeColumn().Component(new AddressComponent("From", Model.SellerAddress));
                    row.ConstantColumn(50);
                    row.RelativeColumn().Component(new AddressComponent("For", Model.CustomerAddress));
                });

                column.Item().Element(ComposeTable);

                var totalPrice = Model.Items.Sum(x => x.Price * x.Quantity);

                column
                    .Item()
                    .PaddingRight(5)
                    .AlignRight()
                    .Text($"Grand total: {totalPrice}$", TextStyle.Default.SemiBold());

                if (!string.IsNullOrWhiteSpace(Model.Comments))
                {
                    column.Item().PaddingTop(25).Element(ComposeComments);
                }
            });
        }

        private void ComposeTable(IContainer container)
        {
            var headerStyle = TextStyle.Default.SemiBold();

            container.Decoration(decoration =>
            {
                // header
                decoration.Header().BorderBottom(1).Padding(5).Row(row =>
                {
                    row.ConstantColumn(25).Text("#", headerStyle);
                    row.RelativeColumn(3).Text("Product", headerStyle);
                    row.RelativeColumn().AlignRight().Text("Unit price", headerStyle);
                    row.RelativeColumn().AlignRight().Text("Quantity", headerStyle);
                    row.RelativeColumn().AlignRight().Text("Total", headerStyle);
                });

                // content
                decoration
                    .Content()
                    .Stack(column =>
                    {
                        foreach (var item in Model.Items)
                        {
                            column
                            .Item()
                            .ShowEntire()
                            .BorderBottom(1)
                            .BorderColor(Colors.Grey.Lighten2)
                            .Padding(5)
                            .Row(row =>
                            {
                                row.ConstantColumn(25).Text(Model.Items.IndexOf(item) + 1);
                                row.RelativeColumn(3).Text(item.Name);
                                row.RelativeColumn().AlignRight().Text($"{item.Price}$");
                                row.RelativeColumn().AlignRight().Text(item.Quantity);
                                row.RelativeColumn().AlignRight().Text($"{item.Price * item.Quantity}$");
                            });
                        }
                    });
            });
        }

        private void ComposeComments(IContainer container)
        {
            container.ShowEntire().Background(Colors.Grey.Lighten3).Padding(10).Stack(message =>
            {
                message.Spacing(5);
                message.Item().Text("Comments", TextStyle.Default.Size(14).SemiBold());
                message.Item().Text(Model.Comments);
            });
        }
    }
}
