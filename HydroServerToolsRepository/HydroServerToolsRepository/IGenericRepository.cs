using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;

using jQuery.DataTables.Mvc;


//Generic repository interface...
namespace HydroServerToolsRepository.Repository
{
    interface IGenericRepository<repositoryType>
    {
        //Methods

        //Return all instances...
        List<repositoryType> GetAll();

        //Return matching instances...
        List<repositoryType> GetInstances(int startIndex,
                                          int pageSize,
                                          ReadOnlyCollection<SortedColumn> sortedColumns,
                                          out int totalRecordCount,
                                          out int searchRecordCount,
                                          string searchString);

        //Add input instances...
        void AddInstances(List<repositoryType> instances,
                          string instanceIdentifier,
                          out List<repositoryType> lstIncorrectInstances,
                          out List<repositoryType> lstCorrectInstances,
                          out List<repositoryType> lstDuplicateInstances,
                          out List<repositoryType> lstEditedInstances);

        //Delete all instances...
        void DeleteAll();
    }
}
