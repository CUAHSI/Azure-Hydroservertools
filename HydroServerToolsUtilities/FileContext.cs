using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;

#if (!USE_BINARY_FORMATTER)
using Newtonsoft.Json;
#endif

//using HydroServerToolsUtilities;

namespace HydroServerToolsUtilities
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
#if (USE_BINARY_FORMATTER)
                        string binIncorrectFilePathAndName = pathProcessed + iUpdateableItems.UploadId + "-" + modelType.Name + "-IncorrectRecords.bin";
#else
                        string binIncorrectFilePathAndName = pathProcessed + iUpdateableItems.UploadId + "-" + modelType.Name + "-IncorrectRecords.json";
#endif
                        using (var fileStream = new FileStream(binIncorrectFilePathAndName, FileMode.Open, FileAccess.Read, FileShare.Read, 65536 * 16, true))
                        {
#if (USE_BINARY_FORMATTER)
                            //De-serialize binary file to generic list...
                            BinaryFormatter binFor = new BinaryFormatter();
                            incorrectItems = (List<UpdateableItem<tModelType>>)binFor.Deserialize(fileStream);
#else
                            //De-serialize JSON file to generic list...
                            using (StreamReader sr = new StreamReader(fileStream))
                            {
                                JsonSerializer jsonSerializer = new JsonSerializer();

                                //incorrectItems = (List<UpdateableItem<tModelType>>)jsonSerializer.Deserialize(sr, typeof(List<UpdateableItem<tModelType>>));
                                using (JsonReader jsonReader = new Newtonsoft.Json.JsonTextReader(sr))
                                {
                                    //Use asynchronous JsonReader method here...
                                    //Source: https://stackoverflow.com/questions/26601594/what-is-the-correct-way-to-use-json-net-to-parse-stream-of-json-objects
                                    jsonReader.SupportMultipleContent = true;
                                    while (await jsonReader.ReadAsync())
                                    {
                                        incorrectItems.Add(jsonSerializer.Deserialize<UpdateableItem<tModelType>>(jsonReader));
                                    }
                                }
                            }
#endif
                        }

                        //Correct records...
#if (USE_BINARY_FORMATTER)
                        string binCorrectFilePathAndName = pathProcessed + iUpdateableItems.UploadId + "-" + modelType.Name + "-CorrectRecords.bin";
#else
                        string binCorrectFilePathAndName = pathProcessed + iUpdateableItems.UploadId + "-" + modelType.Name + "-CorrectRecords.json";
#endif
                        using (var fileStream = new FileStream(binCorrectFilePathAndName, FileMode.Open, FileAccess.Read, FileShare.Read, 65536 * 16, true))
                        {
#if (USE_BINARY_FORMATTER)
                            //De-serialize binary file to generic list...
                            BinaryFormatter binFor = new BinaryFormatter();
                            correctItems = (List<UpdateableItem<tModelType>>)binFor.Deserialize(fileStream);
#else
                            //De-serialize JSON file to generic list...
                            using (StreamReader sr = new StreamReader(fileStream))
                            {
                                JsonSerializer jsonSerializer = new JsonSerializer();

                                //correctItems = (List<UpdateableItem<tModelType>>)jsonSerializer.Deserialize(sr, typeof(List<UpdateableItem<tModelType>>));
                                using (JsonReader jsonReader = new Newtonsoft.Json.JsonTextReader(sr))
                                {
                                    //Use asynchronous JsonReader method here...
                                    //Source: https://stackoverflow.com/questions/26601594/what-is-the-correct-way-to-use-json-net-to-parse-stream-of-json-objects
                                    jsonReader.SupportMultipleContent = true;
                                    while (await jsonReader.ReadAsync())
                                    {
                                        correctItems.Add(jsonSerializer.Deserialize<UpdateableItem<tModelType>>(jsonReader));
                                    }
                                }
                            }
#endif
                        }

                        //Edited records...
#if (USE_BINARY_FORMATTER)
                        string binEditedFilePathAndName = pathProcessed + iUpdateableItems.UploadId + "-" + modelType.Name + "-EditedRecords.bin";
#else
                        string binEditedFilePathAndName = pathProcessed + iUpdateableItems.UploadId + "-" + modelType.Name + "-EditedRecords.json";
#endif
                        using (var fileStream = new FileStream(binEditedFilePathAndName, FileMode.Open, FileAccess.Read, FileShare.Read, 65536 * 16, true))
                        {
#if (USE_BINARY_FORMATTER)
                            //De-serialize binary file to generic list...
                            BinaryFormatter binFor = new BinaryFormatter();
                            editedItems = (List<UpdateableItem<tModelType>>)binFor.Deserialize(fileStream);
#else
                            //De-serialize JSON file to generic list...
                            using (StreamReader sr = new StreamReader(fileStream))
                            {
                                JsonSerializer jsonSerializer = new JsonSerializer();

                                //editedItems = (List<UpdateableItem<tModelType>>)jsonSerializer.Deserialize(sr, typeof(List<UpdateableItem<tModelType>>));
                                using (JsonReader jsonReader = new Newtonsoft.Json.JsonTextReader(sr))
                                {
                                    //Use asynchronous JsonReader method here...
                                    //Source: https://stackoverflow.com/questions/26601594/what-is-the-correct-way-to-use-json-net-to-parse-stream-of-json-objects
                                    jsonReader.SupportMultipleContent = true;
                                    while (await jsonReader.ReadAsync())
                                    {
                                        editedItems.Add(jsonSerializer.Deserialize<UpdateableItem<tModelType>>(jsonReader));
                                    }
                                }
                            }
#endif
                        }

                        //Duplicated records...
#if (USE_BINARY_FORMATTER)
                        string binDuplicatedFilePathAndName = pathProcessed + iUpdateableItems.UploadId + "-" + modelType.Name + "-DuplicateRecords.bin";
#else
                        string binDuplicatedFilePathAndName = pathProcessed + iUpdateableItems.UploadId + "-" + modelType.Name + "-DuplicateRecords.json";
#endif
                        using (var fileStream = new FileStream(binDuplicatedFilePathAndName, FileMode.Open, FileAccess.Read, FileShare.Read, 65536 * 16, true))
                        {
#if (USE_BINARY_FORMATTER)
                            //De-serialize binary file to generic list...
                            BinaryFormatter binFor = new BinaryFormatter();
                            duplicatedItems = (List<UpdateableItem<tModelType>>)binFor.Deserialize(fileStream);
#else
                            //De-serialize JSON file to generic list...
                            using (StreamReader sr = new StreamReader(fileStream))
                            {
                                JsonSerializer jsonSerializer = new JsonSerializer();

                                //duplicatedItems = (List<UpdateableItem<tModelType>>)jsonSerializer.Deserialize(sr, typeof (List<UpdateableItem<tModelType>>));
                                using (JsonReader jsonReader = new Newtonsoft.Json.JsonTextReader(sr))
                                {
                                    //Use asynchronous JsonReader method here...
                                    //Source: https://stackoverflow.com/questions/26601594/what-is-the-correct-way-to-use-json-net-to-parse-stream-of-json-objects
                                    jsonReader.SupportMultipleContent = true;
                                    while (await jsonReader.ReadAsync())
                                    {
                                        duplicatedItems.Add(jsonSerializer.Deserialize<UpdateableItem<tModelType>>(jsonReader));
                                    }
                                }
                            }
#endif
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

                        //Serialize Incorrect records...
                        using (var fileStream = new FileStream(binIncorrectFilePathAndName, FileMode.Create, FileAccess.Write, FileShare.None, 65536 * 16, true))
                        {
#if (USE_BINARY_FORMATTER)
                            //To binary file... 
                            BinaryFormatter binFor = new BinaryFormatter();
                            binFor.Serialize(fileStream, incorrectItems);

                            fileStream.Flush();
#else
                            //To JSON file...
                            using (StreamWriter sw = new StreamWriter(fileStream))
                            {
                                //JsonSerializer jsonSerializer = new JsonSerializer();
                                //jsonSerializer.Serialize(sw, incorrectItems);

                                //fileStream.Flush();
                                using (JsonTextWriter jtw = new JsonTextWriter(sw))
                                {
                                    JsonSerializer jsonSerializer = new JsonSerializer();
                                    //jsonSerializer.Serialize(jtw, incorrectItems);
                                    foreach (var incorrectItem in incorrectItems)
                                    {
                                        jsonSerializer.Serialize(jtw, incorrectItem);
                                    }

                                    await jtw.FlushAsync();
                                }
                            }
#endif
                        }

                        //Correct records...
                        using (var fileStream = new FileStream(binCorrectFilePathAndName, FileMode.Create, FileAccess.Write, FileShare.None, 65536 * 16, true))
                        {
#if (USE_BINARY_FORMATTER)
                            //To binary file... 
                            BinaryFormatter binFor = new BinaryFormatter();
                            binFor.Serialize(fileStream, correctItems);

                            fileStream.Flush();
#else
                            //To JSON file...
                            using (StreamWriter sw = new StreamWriter(fileStream))
                            {
                                //JsonSerializer jsonSerializer = new JsonSerializer();
                                //jsonSerializer.Serialize(sw, correctItems);

                                //fileStream.Flush();
                                using (JsonTextWriter jtw = new JsonTextWriter(sw))
                                {
                                    JsonSerializer jsonSerializer = new JsonSerializer();
                                    //jsonSerializer.Serialize(jtw, correctItems);
                                    foreach (var correctItem in correctItems)
                                    {
                                        jsonSerializer.Serialize(jtw, correctItem);
                                    }

                                    await jtw.FlushAsync();
                                }
                            }
#endif
                        }

                        //Duplicated records...
                        using (var fileStream = new FileStream(binDuplicatedFilePathAndName, FileMode.Create, FileAccess.Write, FileShare.None, 65536 * 16, true))
                        {
#if (USE_BINARY_FORMATTER)
                            //To binary file... 
                            BinaryFormatter binFor = new BinaryFormatter();
                            binFor.Serialize(fileStream, duplicatedItems);

                            fileStream.Flush();
#else
                            //To JSON file...
                            using (StreamWriter sw = new StreamWriter(fileStream))
                            {
                                //JsonSerializer jsonSerializer = new JsonSerializer();
                                //jsonSerializer.Serialize(sw, duplicatedItems);

                                //fileStream.Flush();
                                using (JsonTextWriter jtw = new JsonTextWriter(sw))
                                {
                                    JsonSerializer jsonSerializer = new JsonSerializer();
                                    //jsonSerializer.Serialize(jtw, duplicatedItems);
                                    foreach (var duplicatedItem in duplicatedItems)
                                    {
                                        jsonSerializer.Serialize(jtw, duplicatedItem);
                                    }

                                    await jtw.FlushAsync();
                                }
                            }
#endif
                        }

                        //Edited records...
                        using (var fileStream = new FileStream(binEditedFilePathAndName, FileMode.Create, FileAccess.Write, FileShare.None, 65536 * 16, true))
                        {
#if (USE_BINARY_FORMATTER)
                            //To binary file... 
                            BinaryFormatter binFor = new BinaryFormatter();
                            binFor.Serialize(fileStream, editedItems);

                            fileStream.Flush();
#else
                            //To JSON file...
                            using (StreamWriter sw = new StreamWriter(fileStream))
                            {
                                //JsonSerializer jsonSerializer = new JsonSerializer();
                                //jsonSerializer.Serialize(sw, editedItems);

                                //fileStream.Flush();
                                using (JsonTextWriter jtw = new JsonTextWriter(sw))
                                {
                                    JsonSerializer jsonSerializer = new JsonSerializer();
                                    //jsonSerializer.Serialize(jtw, editedItems);
                                    foreach (var editedItem in editedItems)
                                    {
                                        jsonSerializer.Serialize(jtw, editedItem);
                                    }

                                    await jtw.FlushAsync();
                                }
                            }
#endif
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