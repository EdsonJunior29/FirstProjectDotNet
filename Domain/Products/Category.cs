using FirstProjectDotNetCore.Endpoints.Categories;
using Flunt.Validations;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Diagnostics.Contracts;

namespace FirstProjectDotNetCore.Domain.Products
{
    public class Category : Entity
    {
        public string Name { get; private set; }
        public bool Active { get; private set; }

        public Category(string name, string createdBy, string editedBy)
        {
            Name = name;
            Active = true;
            CreatedBy = createdBy;
            EditedBy = editedBy;
            CreatedOn = DateTime.Now;
            EditedOn = DateTime.Now;

            Validate();
        }

        public void EditCategory(string name, bool active, string editedBy)
        {
            Active = active;
            Name = name;
            EditedOn = DateTime.Now;
            EditedBy = editedBy;
            EditedOn = DateTime.Now;

            Validate();
        }

        private void Validate()
        {
            var contract = new Contract<Category>()
               .IsNotNullOrEmpty(Name, "Name")
               .IsNotNullOrEmpty(CreatedBy, "CreatedBy")
               .IsGreaterOrEqualsThan(Name, 2, "Name")
               .IsNotNullOrEmpty(EditedBy, "EditedBy");
            AddNotifications(contract);
        }
    }
}
