using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.Entity;

namespace HydroServerToolsEFDerivedObjects.Interfaces
{
    public enum FieldType { ftSearching, ftDuplicatesCheck, ftUpdatesCheck };

    //A 'helper' interface implemented by MVC ...Model instances in upload checking...
    public interface ICuahsiUpload<odmType> where odmType: class
    {
        //Return a list of field names per the input field type...
        List<string> GetFields(FieldType ft);

        //Return a dictionary of field positions, names for use in sort processing...
        Dictionary<int, string> GetFieldsForSorting();

        //Copy contents to the specified EntityFramework model type...
        bool CopyTo(odmType odmT, StringBuilder sbError);

        //Per the input DbSet instance, remove all records...
        bool QueueDeleteAll(DbSet<odmType> dbset);
    }
}
