using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PharmYdm.Models
{
    public class PharmYdmQamaClient
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }
        [Display(Name ="מזהה לקוח PharmYdm")]
        public string CustName { get; set; }
        [Display(Name ="מפתח API")]
        public string ApiKey{ get; set; }
    }
}