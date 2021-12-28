using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace BlazorQuestPDF
{
    public class AddressComponent : IComponent
    {
        private string Title { get; }
        private Address Address { get; }

        public AddressComponent(string title, Address address)
        {
            Title = title;
            Address = address;
        }

        public void Compose(IContainer container)
        {
            container.ShowEntire().Stack(column =>
            {
                column.Spacing(5);

                column
                    .Item()
                    .BorderBottom(1)
                    .PaddingBottom(5)
                    .Text(Title, TextStyle.Default.SemiBold());

                column.Item().Text(Address.CompanyName);
                column.Item().Text(Address.Street);
                column.Item().Text($"{Address.City}, {Address.State}");
                column.Item().Text(Address.Email);
                column.Item().Text(Address.Phone);
            });
        }
    }
}
