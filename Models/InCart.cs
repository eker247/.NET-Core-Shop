// Models/InCart.cs
using System;

namespace sklep.Models
{
    public class InCart
    {
        public int Id { get; set; }

        public int IdCart { get; set; }

        public int IdProduct { get; set; }

        public int Number { get; set; }
    }
}