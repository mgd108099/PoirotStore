using System.ComponentModel.DataAnnotations.Schema;

namespace CrimeGameStore.Web.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order? Order { get; set; }

        public int CrimeCaseId { get; set; }
        public CrimeCase? CrimeCase { get; set; }

        public EditionType Edition { get; set; } // Physical أو Digital

        public int Quantity { get; set; } = 1;

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }
    }
}