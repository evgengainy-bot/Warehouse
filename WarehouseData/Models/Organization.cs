using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace WarehouseData.Models
{
    [Table("orgs")]
    public class Organization
    {
        public static BigInteger OrgCounter = BigInteger.Zero;

        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public BigInteger OrgId { get; set; }

        [Column("orgname")]
        public string OrgName { get; set; }

        public ObservableCollection<Warehouse> warehouses { get; set; }
            = new ObservableCollection<Warehouse>();

        public Organization(string name)
        {
            OrgId = ++OrgCounter;
            OrgName = name;
        }

        public override string ToString()
        {
            return OrgName;
        }
    }
}