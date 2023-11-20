using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PharmYdm.Models
{
    public class PharmYdm_Log
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Display(Name="תאריך סנכרון")]
        public DateTime Date { get; set; }

        [Display(Name="מזהה פריוריטי")]
        public string PriorityID { get; set; }

        [Display(Name="מזהה משלוח")]
        public string HfdID { get; set; }

        [Display(Name="מדבקה")]
        public string Stiker { get; set; }

        [Display(Name="פירוט מידע")]
        public string Parameters { get; set; }

        [Display(Name="הצליח?")]
        public bool Success { get; set; } = true;

        [Display(Name="שגיאה")]
        public string Error { get; set; }

    }
}