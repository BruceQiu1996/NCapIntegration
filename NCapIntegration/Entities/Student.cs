using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace NCapIntegration.Entities
{
    [PrimaryKey("Id")]
    [Table("demo-students")]
    public class Student : IEFEntity<int>, IEFSoftDelete
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Birthday { get; set; }
        public bool Sex { get; set; }
        public string Address { get; set; }
        public bool IsDeleted { get; set; }
    }
}
