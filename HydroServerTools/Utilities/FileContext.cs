using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;

using HydroServerToolsUtilities;

namespace HydroServerTools.Utilities
{
    //A simple class for the association of a file name and a file (MIME) type
    public class FileNameAndType
    {
        //Constructors...

        //Default...
        private FileNameAndType() { }

        //Initializing
        public FileNameAndType( string fileName, string fileType)
        {
#if (DEBUG)
            if ( String.IsNullOrWhiteSpace(fileName) || String.IsNullOrWhiteSpace(fileType))
            {
                var paramName = String.IsNullOrWhiteSpace(fileName) ? "fileName" : "fileType";
                throw new ArgumentException("Invalid input parameter...", paramName);
            }
#endif
            this.fileName = fileName;
            this.fileType = fileType;
        }

        //Properties...
        public string fileName { get; set; }

        public string fileType { get; set; }
    }


    //A simple class for the association of prefixed file names with an access semaphore...
    public class FileContext
    {
        //Constructors...

        //Default...
        private FileContext()
        {
            FileNamesAndTypes = new List<FileNameAndType>();
            FileSemaphore = new SemaphoreSlim(1, 1);
        }

        //Initializing constructor...
        public FileContext( string fileNamePrefix, List<FileNameAndType> fileNamesAndTypes) : this()
        {
#if (DEBUG)
            //Validate/initialize input parameters...
            if (String.IsNullOrWhiteSpace(fileNamePrefix) || null == fileNamesAndTypes || 0 >= fileNamesAndTypes.Count)
            {
                string paramName = String.IsNullOrWhiteSpace(fileNamePrefix) ? "fileNamePrefix" : "fileNamesAndTypes";
                throw new ArgumentException("Invalid input parameter...", paramName);
            }
#endif
            //Update instance member(s)...
            FileNamePrefix = fileNamePrefix;

            foreach (var fileNameAndType in fileNamesAndTypes)
            {
                FileNamesAndTypes.Add( new FileNameAndType(fileNameAndType.fileName, fileNameAndType.fileType));
            }
        }

        //Properties...
        private string FileNamePrefix { get; set; }

        public List<FileNameAndType> FileNamesAndTypes { get; private set; }

        public SemaphoreSlim FileSemaphore { get; private set; }

        //Methods...

        //Return a prefixed file name (success) or the empty string (failure)
        public string PrefixedFileName(string fileName)
        {
            string result = String.Empty;
            if (! String.IsNullOrWhiteSpace(fileName))
            {
                foreach (var fileNameAndType in FileNamesAndTypes)
                {
                    if (fileNameAndType.fileName == fileName)
                    {
                        result = FileNamePrefix + "-" + fileName;
                        break;
                    }
                }
            }

            //Processing complete - return result
            return result;
        }

        //Revise contents of binary files per input itemId lists
        //ASSUMPTION: Called after re-load processing to: 
        //              - Remove corrected items from 'incorrect' records binary
        //              - Add corrected items to 'correct', 'updated' or 'duplicate' records binary, as indicated
        //              - Update any 'still incorrect after update' records in 'incorrect' records binary
        public async Task UpdateBinaryFiles<tModelType>(IUpdateableItemsData<tModelType> iUpdateableItems,
                                                        string pathProcessed,
                                                        TableUpdateResult tableUpdateResult)
        {
            //Validate/initialize input parameters...
            if (null != iUpdateableItems && (!String.IsNullOrWhiteSpace(pathProcessed)) && null != tableUpdateResult)
            {
                //Input parameters valid - create updateable items list... 
                Type modelType = typeof(tModelType);

                List<int> correctItemIds = tableUpdateResult.CorrectItemIds;
                List<int> duplicatedItemIds = tableUpdateResult.DuplicateItemIds;
                List<int> editedItemIds = tableUpdateResult.EditedItemIds;
                List<int> incorrectItemIds = tableUpdateResult.IncorrectItemIds;

                List<UpdateableItem<tModelType>> updateableItems = iUpdateableItems.UpdateableItems;

                List<UpdateableItem<tModelType>> incorrectItems = null;
                List<UpdateableItem<tModelType>> correctItems = null;
                List<UpdateableItem<tModelType>> editedItems = null;
                List<UpdateableItem<tModelType>> duplicatedItems = null;

                using (await FileSemaphore.UseWaitAsync())
                {
                    try
                    {
                        //De-serialize binary file contents - Incorrect records...
                        string binIncorrectFilePathAndName = pathProcessed + iUpdateableItems.UploadId + "-" + modelType.Name + "-IncorrectRecords.bin";
                        using (var fileStream = new FileStream(binIncorrectFilePathAndName, FileMode.Open, FileAccess.Read, FileShare.Read, 65536, true))
                        {
                            //De-serialize to generic list...
                            BinaryFormatter binFor = new BinaryFormatter();
                            incorrectItems = (List<UpdateableItem<tModelType>>)binFor.Deserialize(fileStream);
                        }

                        //Correct records...
                        string binCorrectFilePathAndName = pathProcessed + iUpdateableItems.UploadId + "-" + modelType.Name + "-CorrectRecords.bin";
                        using (var fileStream = new FileStream(binCorrectFilePathAndName, FileMode.Open, FileAccess.Read, FileShare.Read, 65536, true))
                        {
                            //De-serialize to generic list...
                            BinaryFormatter binFor = new BinaryFormatter();
                            correctItems = (List<UpdateableItem<tModelType>>)binFor.Deserialize(fileStream);
                        }

                        //Edited records...
                        string binEditedFilePathAndName = pathProcessed + iUpdateableItems.UploadId + "-" + modelType.Name + "-EditedRecords.bin";
                        using (var fileStream = new FileStream(binEditedFilePathAndName, FileMode.Open, FileAccess.Read, FileShare.Read, 65536, true))
                        {
                            //De-serialize to generic list...
                            BinaryFormatter binFor = new BinaryFormatter();
                            editedItems = (List<UpdateableItem<tModelType>>)binFor.Deserialize(fileStream);
                        }

                        //Duplicated records...
                        string binDuplicatedFilePathAndName = pathProcessed + iUpdateableItems.UploadId + "-" + modelType.Name + "-DuplicateRecords.bin";
                        using (var fileStream = new FileStream(binDuplicatedFilePathAndName, FileMode.Open, FileAccess.Read, FileShare.Read, 65536, true))
                        {
                            //De-serialize to generic list...
                            BinaryFormatter binFor = new BinaryFormatter();
                            duplicatedItems = (List<UpdateableItem<tModelType>>)binFor.Deserialize(fileStream);
                        }

                        //Remove all processed items from the incorrect items...
                        incorrectItems.RemoveAll(item => ((-1 != correctItemIds.IndexOf(item.ItemId)) ||
                                                          (-1 != duplicatedItemIds.IndexOf(item.ItemId)) ||
                                                          (-1 != editedItemIds.IndexOf(item.ItemId)) ||
                                                          (-1 != incorrectItemIds.IndexOf(item.ItemId))));

                        //Add newly corrected items to correct items...
                        correctItems.AddRange(updateableItems.Where(item => (-1 != correctItemIds.IndexOf(item.ItemId))));
                        //Add newly duplicated items to duplicated items
                        duplicatedItems.AddRange(updateableItems.Where(item => (-1 != duplicatedItemIds.IndexOf(item.ItemId))));
                        //Add newly edited items to edited items
                        editedItems.AddRange(updateableItems.Where(item => (-1 != editedItemIds.IndexOf(item.ItemId))));
                        //Add newly incorrect items to incorrect items
                        incorrectItems.AddRange(updateableItems.Where(item => (-1 != incorrectItemIds.IndexOf(item.ItemId))));

                        //Serialize binary file contents - Incorrect records...
                        using (var fileStream = new FileStream(binIncorrectFilePathAndName, FileMode.Create, FileAccess.Write, FileShare.None, 65536, true))
                        {
                            BinaryFormatter binFor = new BinaryFormatter();
                            binFor.Serialize(fileStream, incorrectItems);
                            fileStream.Flush();
                        }

                        //Correct records...
                        using (var fileStream = new FileStream(binCorrectFilePathAndName, FileMode.Create, FileAccess.Write, FileShare.None, 65536, true))
                        {
                            BinaryFormatter binFor = new BinaryFormatter();
                            binFor.Serialize(fileStream, correctItems);
                            fileStream.Flush();
                        }

                        //Duplicated records...
                        using (var fileStream = new FileStream(binDuplicatedFilePathAndName, FileMode.Create, FileAccess.Write, FileShare.None, 65536, true))
                        {
                            BinaryFormatter binFor = new BinaryFormatter();
                            binFor.Serialize(fileStream, duplicatedItems);
                            fileStream.Flush();
                        }

                        //Edited records...
                        using (var fileStream = new FileStream(binEditedFilePathAndName, FileMode.Create, FileAccess.Write, FileShare.None, 65536, true))
                        {
                            BinaryFormatter binFor = new BinaryFormatter();
                            binFor.Serialize(fileStream, editedItems);
                            fileStream.Flush();
                        }
                    }
                    catch (Exception ex)
                    {
                        //File not found - return early
                        string msg = ex.Message;

                        return;
                    }
                }
            }
        }
    }
}