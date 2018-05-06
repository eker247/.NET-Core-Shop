// Models/Cart.cs
using System;

namespace sklep.Models
{
    public class Cart
    {
        public int Id { get; set; }

        public string IdUser { get; set; }

        public DateTime Date { get; set; }

        public Cart() {}

        public Cart(int id, string iduser, DateTime date)
        {
            Id = id;
            IdUser = iduser;
            Date = date;
        }
    }
}