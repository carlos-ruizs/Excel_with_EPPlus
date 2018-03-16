using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using OfficeOpenXml;
using System.IO;
using System.Xml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Style.XmlAccess;
using System.Linq;
using System.Data;
using System.Threading.Tasks;
using System.Diagnostics;

namespace PruebaExcel_EPplus
{

    class libExcel_epp
    {
        //atributes
        public ExcelPackage m_objExcel; //the Excel app itself
        public ExcelWorkbook m_objWorkbook; //A workbook object we will be using constantly
        public ExcelWorksheet m_objWorksheet; //A worksheet object because we need to create at least one worksheet
        private string m_strFileName; //the name of the file we are using, this is used for Workbooks as those are the files themselves
        public FileInfo m_fileInfo; //the path of the directory where we want to save the file
        private string m_strFontName; //the name of the font we want to use
        private int m_intFontSize; //the size of the font we want to use
        Dictionary<KeyValuePair<int, int>, object> values;
        bool m_bTitle = false;


        //properties
        public FileInfo ProprFileInfo { get => m_fileInfo; set => m_fileInfo = value; }
        private string ProprFileName { get => m_strFileName; set => m_strFileName = value; }
        private string ProprFontName { get => m_strFontName; set => m_strFontName = value; }
        private int ProprFontSize { get => m_intFontSize; set => m_intFontSize = value; }
        public string ID { get; set; }
        public string Num { get; set; }
        public string String { get; set; }

        //methods
        public libExcel_epp() // default constructor
        {
            this.m_objExcel = new ExcelPackage();
        }

        public libExcel_epp(string pstrWorkbookName) // Constructor that receives the name of the Woorkbook as it's parameter
        {
            this.m_objExcel = new ExcelPackage();
            this.m_strFileName = pstrWorkbookName;
            WorkbookCreate(m_strFileName);
            m_objExcel.Save();
        }

        public libExcel_epp(string pstrWorkbookName, string pstrWorksheetName) // Constructor that receives the name of the Workbook and a Worksheet to start
        {
            this.m_objExcel = new ExcelPackage();
            this.m_strFileName = pstrWorkbookName;
            WorkbookCreate(m_strFileName, pstrWorksheetName);
            m_objExcel.Save();
        }

        public void WorkbookCreate() //creates a Workbook with a default name for both the file and the first worksheet
        {
            /**
            var newFile = new FileInfo(outputDirectory.FullName + @"\Workbook.xlsx");
            if (newFile.Exists)
            {
                newFile.Delete();  // ensures we create a new workbook
                newFile = new FileInfo(outputDirectory.FullName + @"\Workbook.xlsx");
            }

            using (var package = new ExcelPackage(newFile));
            */
            m_objExcel.Workbook.Worksheets.Add("Sheet 1");
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); //this line is used to encode the file so that it can be saved where we want to
            m_objExcel.SaveAs(m_fileInfo = new FileInfo(@"E:\Workbook.xlsx"));
            m_objExcel.Save();
        }

        public void WorkbookCreate(string pstrWorkbookName, string pstrWorksheetName) // creates a new Workbook with both the name of the Workbook and the name of the first worksheet
        {
            FileInfo file = new FileInfo(@"E:\" + pstrWorkbookName + ".xlsx");
            if (file.Exists)
            {
                Console.WriteLine("Existing archive. Overwriting");
                OverwriteExcel(pstrWorkbookName);
            }
            else
            {
                m_objExcel.Workbook.Worksheets.Add(pstrWorksheetName);
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                m_objExcel.SaveAs(m_fileInfo = new FileInfo(@"E:\" + pstrWorkbookName + ".xlsx"));
                m_objExcel.Save();
            }

        }

        public void WorkbookCreate(string pstrWorkbookName) //creates a new Workbook with a specified name, the first worksheet is named "Sheet 1"
        {
            m_objExcel.Workbook.Worksheets.Add("Sheet 1");
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            m_objExcel.SaveAs(m_fileInfo = new FileInfo(@"E:\" + pstrWorkbookName + ".xlsx"));
            m_objExcel.Save();
        }

        public void WorkbookCreate(string pstrRoute, string pstrWorkbookName, string pstrWorksheetName) //creates a new Workbook with a specified route, Workbook name and worksheet name
        {
            m_objExcel.Workbook.Worksheets.Add(pstrWorksheetName);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            m_objExcel.SaveAs(m_fileInfo = new FileInfo(pstrRoute + pstrWorkbookName + ".xlsx")); //for now, it only saves to an already existing directory
            m_objExcel.Save();
        }

        public void WorkbookDelete(string pstrWorkbookName) //deletes a Workbook with a specified name
        {
            FileInfo file = new FileInfo(@"E:\" + pstrWorkbookName + ".xlsx");
            if (file.Exists)
                file.Delete();
        }

        public void WorkbookDelete(string pstrRoute, string pstrWorkbookName) //deletes a Workbook in a specified route with a specified name
        {
            FileInfo file = new FileInfo(pstrRoute + pstrWorkbookName + ".xlsx");

            if (file.Exists)
                file.Delete();
        }


        //Creates a Worksheet for an existing Workbook (for some reason it can only do that if the Workbook is immediatly created before this is called. Weird)
        //update 2/26/2018 Now I know why: because you have to create the object first so you can manipulate it at run-time
        public void WorksheetCreate(string pstrWorksheetName)
        {
            ExcelWorksheet worksheet = m_objExcel.Workbook.Worksheets.Add(pstrWorksheetName);
            m_strFileName = m_fileInfo.FullName;
            Console.WriteLine("Agregando una nueva worksheet a: " + m_strFileName);
            Console.ReadKey();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            m_objExcel.SaveAs(m_fileInfo);
            m_objExcel.Save();
        }

        //This will be here for now 
        public void OverwriteExcel(string pstrFileName)
        {
            m_objExcel.File.Replace(pstrFileName, pstrFileName);
            m_objExcel.Save();
        }

        //Added the ability to open a file from a FileStream and modify it, so I did what I wanted to do for a while which was edit an existing file
        //this should take the name of a Worksheet and at some styling and format to it. Either or both.+, I'll check what I can do
        public void WorksheetStyling(string pstrWorksheetName, int pintFontSize, string pstrFontName)
        {
            using (FileStream stream = new FileStream(@"E:\WorkbookPrueba.xlsx", FileMode.Open))
            {
                m_objExcel.Load(stream);
                ExcelWorksheet worksheet = m_objExcel.Workbook.Worksheets[pstrWorksheetName];
                worksheet.TabColor = Color.Blue;
                worksheet.DefaultRowHeight = 12;

                using (var range = worksheet.Cells[1, 1, 1, 5])
                {
                    range.Style.Font.Size = pintFontSize;
                    range.Style.Font.Name = pstrFontName;
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.Black);
                    range.Style.Font.Color.SetColor(Color.White);
                    range.Style.ShrinkToFit = false;
                }

                using (var range = worksheet.Cells[2, 1, 5, 5])
                {
                    range.Style.Font.Size = pintFontSize;
                    range.Style.Font.Name = pstrFontName;
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.Gray);
                    range.Style.Font.Color.SetColor(Color.WhiteSmoke);
                    range.Style.ShrinkToFit = false;
                }
            }
            m_objExcel.Save();
        }

        public void WorksheetStyling(string pstrWorkbookName, string pstrWorksheetName, int pintFontSize, string pstrFontName)
        {
            using (FileStream stream = new FileStream(@"E:\" + pstrWorkbookName + ".xlsx", FileMode.Open))
            {
                m_objExcel.Load(stream);
                ExcelWorksheet worksheet = m_objExcel.Workbook.Worksheets[pstrWorksheetName];

                worksheet.DefaultRowHeight = 12;
                worksheet.TabColor = Color.OrangeRed;

                using (var range = worksheet.Cells[1, 1, 1, 5])
                {
                    range.Style.Font.Size = pintFontSize;
                    range.Style.Font.Name = pstrFontName;
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.Black);
                    range.Style.Font.Color.SetColor(Color.White);
                    range.Style.ShrinkToFit = false;
                }

                using (var range = worksheet.Cells[2, 1, 5, 5])
                {
                    range.Style.Font.Size = pintFontSize;
                    range.Style.Font.Name = pstrFontName;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.Gray);
                    range.Style.Font.Color.SetColor(Color.BlueViolet);
                }
            }
            m_objExcel.Save();
        }

        public void WorksheetStyling(string pstrRoute, string pstrWorkbookName, string pstrWorksheetName, int pintFontSize, string pstrFontName)
        {
            using (FileStream stream = new FileStream(pstrRoute + pstrWorkbookName + ".xlsx", FileMode.Open))
            {
                m_objExcel.Load(stream);
                ExcelWorksheet worksheet = m_objExcel.Workbook.Worksheets[pstrWorksheetName];

                worksheet.DefaultRowHeight = 12;
                worksheet.TabColor = Color.OrangeRed;

                using (var range = worksheet.Cells[1, 1, 1, 5])
                {
                    range.Style.Font.Size = pintFontSize;
                    range.Style.Font.Name = pstrFontName;
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.Gray);
                    range.Style.Font.Color.SetColor(Color.Black);
                    range.Style.ShrinkToFit = false;
                }

                using (var range = worksheet.Cells[2, 1, 5, 5])
                {
                    range.Style.Font.Size = pintFontSize;
                    range.Style.Font.Name = pstrFontName;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.Aquamarine);
                    range.Style.Font.Color.SetColor(Color.Violet);
                }
            }
            m_objExcel.Save();
        }



        public void DataManipulation(string pstrWorkbookName, string pstrWorksheetName)
        {
            using (FileStream stream = new FileStream(@"E:\" + pstrWorkbookName + ".xlsx", FileMode.Open))
            {
                using (ExcelPackage excelObj = new ExcelPackage())
                {
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    excelObj.Load(stream);
                    ExcelWorksheet worksheet = excelObj.Workbook.Worksheets[pstrWorksheetName];
                    Dictionary<string, int[]> worksheetContent = new Dictionary<string, int[]>(); //key is a string, value is an int
                    var worksheetEnd = worksheet.Dimension.End; //Gives me the end of the worksheet, which it appears is where the last value was written
                    var worksheetStart = worksheet.Dimension.Start; //Gives me the start of the worksheet, so it will probably be A1
                    //var table = worksheet.C
                    Console.WriteLine("The worksheet starts at {0} and ends at {1} ",worksheetStart.Address,worksheetEnd.Address);
                    Console.ReadKey();

                    for (int rowIndex = worksheet.Dimension.Start.Row; rowIndex <= worksheet.Dimension.End.Row; rowIndex++)
                    {
                        for (int colIndex = worksheet.Dimension.Start.Column; colIndex <= worksheet.Dimension.End.Column; colIndex++)
                        {
                            if (worksheet.Cells[rowIndex, colIndex].Value != null) //if the value in a specific cell isn't null, then
                            {
                                string columnName = worksheet.Cells[rowIndex, colIndex].Value.ToString(); //gets the value in a cell and transformrs it into a string
                                
                                worksheetContent.Add(columnName, new int[] {rowIndex, colIndex}); //adds the cell value in a specific column to the dictionary
                            }
                        }
                    }
                     
                    foreach(KeyValuePair<string, int[]> kvp in worksheetContent){ //for every pair in the dictionary
                        //Debug.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value); //write the pairs in a console output
                        Console.WriteLine("KEY is : " + kvp.Key +
                    "  VALUE is : " + String.Join(",", kvp.Value));
                    }
                    Console.ReadKey(); //wait for the user to input something before finishing 

                    
                    if (worksheetContent.TryGetValue("1", out int[] values))
                    {
                        Console.WriteLine("PRUEBAAA: " + String.Join(",",values));
                        Console.ReadKey();
                    }
                }

                
            }
        }

        public void HeaderAsKey(string pstrWorkbookName, string pstrWorksheetName)
        {
            using (FileStream stream = new FileStream(@"E:\" + pstrWorkbookName + ".xlsx", FileMode.Open))
            {
                using (ExcelPackage objExcel = new ExcelPackage())
                {
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    objExcel.Load(stream);
                    ExcelWorksheet worksheet = objExcel.Workbook.Worksheets[pstrWorksheetName];
                    Dictionary<string, List<string>> worksheetContent = new Dictionary<string, List<string>>(); //A dictionary where the column names are the keys and the values inside the cells are saved in a list
                    List<string> keyValues = new List<string>();
                    List<string> valNames = new List<string>();

                    for (int colIndex = worksheet.Dimension.Start.Column; colIndex <= worksheet.Dimension.End.Column; colIndex++) //iterates through the names of the columns which act as headers
                    {
                        string columnName = worksheet.Cells[1, colIndex].Value.ToString(); //takes the names of the columns and converts them to a string
                        keyValues.Add(columnName);//Adds the names to the list
                    }

                    Console.WriteLine("List of keys of the would be dictionary: ");
                    foreach (string key in keyValues)
                    {
                        Console.WriteLine(key);
                    }
                    Console.ReadKey();
                    Console.Clear();

                    string Dimension = worksheet.Dimension.Address;
                    int rowCount = worksheet.Dimension.Rows;
                    int colCount = worksheet.Dimension.Columns;

                    Console.WriteLine("The dimension of the Worksheet: {0}", Dimension);
                    Console.WriteLine("There are {0} rows and {1} columns being used in the worksheet", rowCount, colCount);
                    Console.ReadKey();
                    Console.Clear();
                    

                    for (int colIndex = worksheet.Dimension.Start.Column; colIndex <= worksheet.Dimension.End.Column; colIndex++)
                    {
                        for (int rowIndex = worksheet.Dimension.Start.Row + 1; rowIndex <= worksheet.Dimension.End.Row; rowIndex++) //this iterates through EVERY row at once, check if I can set the dimension of the rows to iterate in a dynamic way
                        {
                            string columnName = worksheet.Cells[rowIndex, colIndex].Value.ToString();
                            valNames.Add(columnName);
                        }
                        worksheetContent.Add(keyValues[colIndex-1],valNames);
                    }

                    Console.WriteLine("List of values inside the cells: ");
                    foreach (string val in valNames)
                    {
                        Console.WriteLine(val);
                    }
                    Console.ReadKey();
                    Console.Clear();

                    Console.WriteLine("The values inside the dictionary are: ");
                    foreach(KeyValuePair<string,List<string>> kvp in worksheetContent)
                    {
                        foreach(string value in kvp.Value)
                        {
                            Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, value);
                        }
                    }
                    Console.ReadKey();

                    /**
                    for (int rowIndex = worksheet.Dimension.Start.Row + 1; rowIndex <= worksheet.Dimension.End.Row; rowIndex++) //iterates from the first element of the first row, to the last element of the last row
                    {
                        for (int colIndex = worksheet.Dimension.Start.Column; colIndex <= worksheet.Dimension.End.Column; colIndex++) //does the same thing as the loop before but for the column values
                        {
                            if (worksheet.Cells[rowIndex, colIndex].Value != null) //if the value in a specific cell isn't null, then
                            {
                               
                                
                            }
                        }
                    }
                    */
                }
            }

        }


        public void Excel_Create(string SheetName, int col1, int col2, string col3, string URL)
        {
            //ExcelPackage ExcelPkg = new ExcelPackage();
            //ExcelWorksheet wsSheet1 = ExcelPkg.Workbook.Worksheets.Add(SheetName);

            //using (ExcelRange Rng = wsSheet1.Cells[row, col])
            //{
            //    Rng.Value = val;
            //    Rng.Style.Font.Size = 16;
            //    Rng.Style.Font.Bold = true;
            //    Rng.Style.Font.Italic = true;
            //}

            var datatable = new DataTable("tblData");
            //Generate titles of datatable
            datatable.Columns.AddRange(new[] { new DataColumn("ID", typeof(int)), new DataColumn("Num", typeof(int)), new DataColumn("String", typeof(object)), new DataColumn("Screenshot", typeof(string)) });
            m_bTitle = true;
            var row = datatable.NewRow();
            row[0] = col1;
            row[1] = col2;
            row[2] = col3 + " " + Path.GetRandomFileName();
            datatable.Rows.Add(row);

            //Create a test file
            var existingFile = new FileInfo(@"D:\FinalTest.xlsx");
            if (existingFile.Exists)
                existingFile.Delete();

            using (var pck = new ExcelPackage(existingFile))
            {
                var worksheet = pck.Workbook.Worksheets.Add(SheetName);
                worksheet.Cells.LoadFromDataTable(datatable, true);
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                pck.Save();
            }

            using (var pck = new ExcelPackage(existingFile))
            {
                var worksheet = pck.Workbook.Worksheets[SheetName];

                //Cells only contains references to cells with actual data
                var cells = worksheet.Cells;
                var dictionary = cells.GroupBy(c => new { c.Start.Row, c.Start.Column })
                    .ToDictionary(
                        rcg => new KeyValuePair<int, int>(rcg.Key.Row, rcg.Key.Column),
                        rcg => cells[rcg.Key.Row, rcg.Key.Column].Value);

                foreach (var kvp in dictionary)
                {
                    Console.WriteLine("{{ Row: {0}, Column: {1}, Value: \"{2}\" }}", kvp.Key.Key, kvp.Key.Value, kvp.Value);
                }
                Console.ReadLine();


                string StyleName = "HyperStyle";
                ExcelNamedStyleXml HyperStyle = worksheet.Workbook.Styles.CreateNamedStyle(StyleName);
                HyperStyle.Style.Font.UnderLine = true;
                HyperStyle.Style.Font.Size = 12;
                HyperStyle.Style.Font.Color.SetColor(Color.Blue);

                //------HYPERLINK to a website.  
                using (ExcelRange Rng = worksheet.Cells[2, 4, 2, 4])
                {
                    Rng.Hyperlink = new Uri("http://" + URL, UriKind.Absolute);
                    Rng.Value = "Screenshot";
                    Rng.StyleName = StyleName;
                }


                worksheet.Protection.IsProtected = false;
                worksheet.Protection.AllowSelectLockedCells = false;
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                pck.SaveAs(new FileInfo(@"D:\TestFinal.xlsx"));
            }
        }

        //This Excel_Mod method it's just for update a single cell  
        public void Excel_Mod(int row, int col, string val)
        {
            FileInfo file = new FileInfo(@"D:\FinalTest.xlsx");
            using (ExcelPackage excelPackage = new ExcelPackage(file))
            {
                ExcelWorkbook excelWorkBook = excelPackage.Workbook;
                ExcelWorksheet excelWorksheet = excelWorkBook.Worksheets[1];
                excelWorksheet.Cells[row, col].Value = val;
                //excelWorksheet.Cells[3, 2].Value = "Test2";
                //excelWorksheet.Cells[3, 3].Value = "Test3";

                excelPackage.Save();
            }
        }

        //This Excel_Mod method it's just for update or add a entire row 
        public void Excel_Mod(int row, int col1, int col2, string col3, string URL)
        {
            FileInfo file = new FileInfo(@"D:\FinalTest.xlsx");
            using (ExcelPackage excelPackage = new ExcelPackage(file))
            {
                ExcelWorkbook excelWorkBook = excelPackage.Workbook;
                ExcelWorksheet excelWorksheet = excelWorkBook.Worksheets[1];
                excelWorksheet.Cells[row, 1].Value = col1;
                excelWorksheet.Cells[row, 2].Value = col2;
                excelWorksheet.Cells[row, 3].Value = col3;
                excelWorksheet.Cells[row, 4].Value = URL;

                //Update the field "Screenshot"
                string StyleName = "HyperStyle";
                //ExcelNamedStyleXml HyperStyle = excelWorksheet.Workbook.Styles.CreateNamedStyle(StyleName);
                //HyperStyle.Style.Font.UnderLine = true;
                //HyperStyle.Style.Font.Size = 12;
                //HyperStyle.Style.Font.Color.SetColor(Color.Blue);
                using (ExcelRange Rng = excelWorksheet.Cells[row, 4, row, 4])
                {
                    Rng.Hyperlink = new Uri("http://" + URL, UriKind.Absolute);
                    Rng.Value = "Screenshot";
                    Rng.StyleName = StyleName;
                }

                excelPackage.Save();
            }
        }

        public DataTable GetDataTableFromExcel(string path, bool hasHeader = true)
        {
            //FileInfo file = new FileInfo(@"D:\FinalTest.xlsx");
            using (ExcelPackage pck = new ExcelPackage())
            {
                FileStream stream = new FileStream(path, FileMode.Open);
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                pck.Load(stream);
               
                var ws = pck.Workbook.Worksheets.First();
                DataTable tbl = new DataTable();
                foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
                {
                    tbl.Columns.Add(hasHeader ? firstRowCell.Text : string.Format("Column {0}", firstRowCell.Start.Column));
                }
                var startRow = hasHeader ? 2 : 1;
                for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                    DataRow row = tbl.Rows.Add();
                    foreach (var cell in wsRow)
                    {
                        row[cell.Start.Column - 1] = cell.Text;
                    }

                    Dictionary<string, double[]> dict = new Dictionary<string, double[]>();
                    for (int i = 0; i < tbl.Columns.Count; i++)
                        dict.Add(tbl.Columns[i].ColumnName, tbl.Rows.Cast<DataRow>().Select(k => Convert.ToDouble(k[tbl.Columns[i]])).ToArray()); //Input string was not cast right

                    foreach (var kvp in dict)
                    {
                        Console.WriteLine("{{ Column: {0}, Value: \"{1}\" }}", kvp.Key, kvp.Value);
                    }
                    Console.ReadLine();

                }
                return tbl;
            }
        }

        /// <summary>
        /// Gets the excel header and creates a dictionary object based on column name in order to get the index.
        /// Assumes that each column name is unique.
        /// </summary>
        /// <param name="workSheet"></param>
        /// <returns></returns>
        public static Dictionary<string, int> GetExcelHeader(ExcelWorksheet workSheet, int rowIndex)
        {
            Dictionary<string, int> header = new Dictionary<string, int>();

            if (workSheet != null)
            {
                for (int columnIndex = workSheet.Dimension.Start.Column; columnIndex <= workSheet.Dimension.End.Column; columnIndex++)
                {
                    if (workSheet.Cells[rowIndex, columnIndex].Value != null)
                    {
                        string columnName = workSheet.Cells[rowIndex, columnIndex].Value.ToString();

                        if (!header.ContainsKey(columnName) && !string.IsNullOrEmpty(columnName))
                        {
                            header.Add(columnName, columnIndex);
                        }

                    }
                }
            }

            return header;
        }

        /// <summary>
        /// Parse worksheet values based on the information given.
        /// </summary>
        /// <param name="workSheet"></param>
        /// <param name="rowIndex"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static string ParseWorksheetValue(ExcelWorksheet workSheet, Dictionary<string, int> header, int rowIndex, string columnName)
        {
            string value = string.Empty;
            int? columnIndex = header.ContainsKey(columnName) ? header[columnName] : (int?)null;

            if (workSheet != null && columnIndex != null && workSheet.Cells[rowIndex, columnIndex.Value].Value != null)
            {
                value = workSheet.Cells[rowIndex, columnIndex.Value].Value.ToString();
            }

            return value;
        }
    }
}
