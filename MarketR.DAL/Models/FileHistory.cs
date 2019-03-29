//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MarketR.DAL.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class FileHistory
    {
        public FileHistory()
        {
            this.FileRecords = new HashSet<FileRecord>();
            this.FileCalculations = new HashSet<FileCalculation>();
            this.NewFileRecords = new HashSet<NewFileRecord>();
            this.NewFileRecordsCSVs = new HashSet<NewFileRecordsCSV>();
            this.NewFileRecords2 = new HashSet<NewFileRecords2>();
        }
    
        public int FileID { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> FileDate { get; set; }
    
        public virtual ICollection<FileRecord> FileRecords { get; set; }
        public virtual ICollection<FileCalculation> FileCalculations { get; set; }
        public virtual ICollection<NewFileRecord> NewFileRecords { get; set; }
        public virtual ICollection<NewFileRecordsCSV> NewFileRecordsCSVs { get; set; }
        public virtual ICollection<NewFileRecords2> NewFileRecords2 { get; set; }
    }
}
