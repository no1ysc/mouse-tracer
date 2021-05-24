using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace analyser
{
    public struct UserPathAndTimeResult
    {
        /// <summary>
        /// User, Question, Section
        /// </summary>
        public UserPathAndTimeItem[,,] perUser;

        /// <summary>
        /// Question, Section
        /// </summary>
        public UserPathAndTimeItem[,] perQuestion;
    }


    public class UserPathAndTime
    {
        public UserPathAndTime(DataContainer dataContainer) => DataContainer = dataContainer;

        public DataContainer DataContainer { get; }

        public UserPathAndTimeResult Result { get; private set; }

        internal UserPathAndTimeResult Calculate(double wheelToDistance, double clickToDistance)
        {
            int userCount = DataContainer.MouseTraces.Count;
            int maxQuestionCount = DataContainer.MouseTraces.Max(s => s.Value.Max(ss => Int32.Parse(ss.Key)));
            int maxSectionCount = DataContainer.MouseTraces.Max(s => s.Value.Max(ss => ss.Value.Max(sss => {
                if (Int32.Parse(sss.Key) > 5)
                {
                    Console.WriteLine($"{s.Key},{ss.Key},{sss.Key}");
                    Console.WriteLine("zzzzzzzzzz");
                }
                return Int32.Parse(sss.Key);
            })));
            //DataContainer.MouseTraces.OrderBy(s => s.Value.OrderBy(ss => ss.Value.OrderBy(sss => sss.Key)));

            UserPathAndTimeResult result = new UserPathAndTimeResult();
            result.perUser = new UserPathAndTimeItem[userCount, maxQuestionCount, maxSectionCount];
            result.perQuestion = new UserPathAndTimeItem[maxQuestionCount, maxSectionCount];

            foreach (var mouseTraceUser in DataContainer.MouseTraces)
            {
                int userIndex = Int32.Parse(mouseTraceUser.Key) - 1;
                foreach (var mouseTraceQuestion in mouseTraceUser.Value)
                {
                    int questionIndex = Int32.Parse(mouseTraceQuestion.Key) - 1;
                    foreach (var mouseTraceSection in mouseTraceQuestion.Value)
                    {
                        int sectionIndex = Int32.Parse(mouseTraceSection.Key) - 1;

                        var mouseTrace = mouseTraceSection.Value;
                        var timeInfo = DataContainer.TimeInformations[mouseTraceUser.Key][mouseTraceQuestion.Key][mouseTraceSection.Key];

                        result.perUser[userIndex, questionIndex, sectionIndex] = new UserPathAndTimeItem(
                            mouseTrace.PathLength,
                            mouseTrace.PathLength + (double)mouseTrace.ClickCount * clickToDistance + (double)mouseTrace.WheelCount * wheelToDistance,
                            (timeInfo.EndTime - timeInfo.StartTime).TotalMilliseconds,
                            mouseTrace.PureOperationTime.TotalMilliseconds
                            );
                    }
                }
            }

            Result = result;

            return Result;
        }

        public void saveDetail(string path)
        {
            Application excelApp = null;
            Workbook workBook = null;
            Worksheet workSheetForMovingDistance = null;
            Worksheet workSheetForWeightMovingDistance = null;
            Worksheet workSheetForOperationMillisecond = null;
            Worksheet workSheetForPureOperationMillisecond = null;
            try
            {
                excelApp = new Application(); // 엑셀 어플리케이션 생성
                workBook = excelApp.Workbooks.Add(); // 워크북 추가


                workSheetForMovingDistance = workBook.Worksheets.get_Item(1) as Worksheet; // 엑셀 첫번째 워크시트 가져오기
                workSheetForMovingDistance.Name = "이동거리";
                workSheetForMovingDistance.Columns.AutoFit(); // 열 너비 자동 맞춤

                workSheetForWeightMovingDistance = (Worksheet)excelApp.Worksheets.Add(Type.Missing, excelApp.Worksheets[excelApp.Worksheets.Count], 1, XlSheetType.xlWorksheet);
                workSheetForWeightMovingDistance.Name = "가중된 이동거리";
                workSheetForWeightMovingDistance.Columns.AutoFit(); // 열 너비 자동 맞춤

                workSheetForOperationMillisecond = (Worksheet)excelApp.Worksheets.Add(Type.Missing, excelApp.Worksheets[excelApp.Worksheets.Count], 1, XlSheetType.xlWorksheet);
                workSheetForOperationMillisecond.Name = "수행시간(밀리초)";
                workSheetForOperationMillisecond.Columns.AutoFit(); // 열 너비 자동 맞춤

                workSheetForPureOperationMillisecond = (Worksheet)excelApp.Worksheets.Add(Type.Missing, excelApp.Worksheets[excelApp.Worksheets.Count], 1, XlSheetType.xlWorksheet);
                workSheetForPureOperationMillisecond.Name = "순수수행시간(밀리초)";
                workSheetForPureOperationMillisecond.Columns.AutoFit(); // 열 너비 자동 맞춤

                // 해더
                workSheetForMovingDistance.Cells[1, 1] = "사용자";
                workSheetForWeightMovingDistance.Cells[1, 1] = "사용자";
                workSheetForOperationMillisecond.Cells[1, 1] = "사용자";
                workSheetForPureOperationMillisecond.Cells[1, 1] = "사용자";
                for (int q = 0; q < 18; q++)
                {
                    for (int s = 0; s < 5; s++)
                    {
                        workSheetForMovingDistance.Cells[1, q * 5 + 2 + s] = $"'{q + 1}-{s + 1}";
                        workSheetForWeightMovingDistance.Cells[1, q * 5 + 2 + s] = $"'{q + 1}-{s + 1}";
                        workSheetForOperationMillisecond.Cells[1, q * 5 + 2 + s] = $"'{q + 1}-{s + 1}";
                        workSheetForPureOperationMillisecond.Cells[1, q * 5 + 2 + s] = $"'{q + 1}-{s + 1}";
                    }
                }

                // 데이터
                for (int u = 0; u < 22; u++)
                {
                    // 사용자 id
                    workSheetForMovingDistance.Cells[u + 2, 1] = $"{u + 1}";
                    workSheetForWeightMovingDistance.Cells[u + 2, 1] = $"{u + 1}";
                    workSheetForOperationMillisecond.Cells[u + 2, 1] = $"{u + 1}";
                    workSheetForPureOperationMillisecond.Cells[u + 2, 1] = $"{u + 1}";
                    for (int q = 0; q < 18; q++)
                    {
                        for (int s = 0; s < 5; s++)
                        {
                            if (Result.perUser[u, q, s] != null)
                            {
                                workSheetForMovingDistance.Cells[u + 2, q * 5 + 2 + s] = Result.perUser[u, q, s].MovingDistance;
                                workSheetForWeightMovingDistance.Cells[u + 2, q * 5 + 2 + s] = Result.perUser[u, q, s].WeightMovingDistance;
                                workSheetForOperationMillisecond.Cells[u + 2, q * 5 + 2 + s] = Result.perUser[u, q, s].OperationMillisecond;
                                workSheetForPureOperationMillisecond.Cells[u + 2, q * 5 + 2 + s] = Result.perUser[u, q, s].PureOperationMillisecond;
                            }
                        }
                    }
                }

                workBook.SaveAs(path, XlFileFormat.xlWorkbookDefault); // 엑셀 파일 저장
                workBook.Close(true); 
                excelApp.Quit(); 
            } finally {
                Marshal.ReleaseComObject(workSheetForMovingDistance);
                Marshal.ReleaseComObject(workSheetForWeightMovingDistance);
                Marshal.ReleaseComObject(workSheetForOperationMillisecond);
                Marshal.ReleaseComObject(workSheetForPureOperationMillisecond);
                Marshal.ReleaseComObject(workBook);
                Marshal.ReleaseComObject(excelApp); 
            }
        }

        public void save(string path)
        {
            Application excelApp = null;
            Workbook workBook = null;
            Worksheet workSheetForMovingDistance = null;
            Worksheet workSheetForWeightMovingDistance = null;
            Worksheet workSheetForOperationMillisecond = null;
            Worksheet workSheetForPureOperationMillisecond = null;
            try
            {
                excelApp = new Application(); // 엑셀 어플리케이션 생성
                workBook = excelApp.Workbooks.Add(); // 워크북 추가


                workSheetForMovingDistance = workBook.Worksheets.get_Item(1) as Worksheet; // 엑셀 첫번째 워크시트 가져오기
                workSheetForMovingDistance.Name = "이동거리";
                workSheetForMovingDistance.Columns.AutoFit(); // 열 너비 자동 맞춤

                workSheetForWeightMovingDistance = (Worksheet)excelApp.Worksheets.Add(Type.Missing, excelApp.Worksheets[excelApp.Worksheets.Count], 1, XlSheetType.xlWorksheet);
                workSheetForWeightMovingDistance.Name = "가중된 이동거리";
                workSheetForWeightMovingDistance.Columns.AutoFit(); // 열 너비 자동 맞춤

                workSheetForOperationMillisecond = (Worksheet)excelApp.Worksheets.Add(Type.Missing, excelApp.Worksheets[excelApp.Worksheets.Count], 1, XlSheetType.xlWorksheet);
                workSheetForOperationMillisecond.Name = "수행시간(밀리초)";
                workSheetForOperationMillisecond.Columns.AutoFit(); // 열 너비 자동 맞춤

                workSheetForPureOperationMillisecond = (Worksheet)excelApp.Worksheets.Add(Type.Missing, excelApp.Worksheets[excelApp.Worksheets.Count], 1, XlSheetType.xlWorksheet);
                workSheetForPureOperationMillisecond.Name = "순수수행시간(밀리초)";
                workSheetForPureOperationMillisecond.Columns.AutoFit(); // 열 너비 자동 맞춤

                // 해더
                workSheetForMovingDistance.Cells[1, 1] = "사용자";
                workSheetForWeightMovingDistance.Cells[1, 1] = "사용자";
                workSheetForOperationMillisecond.Cells[1, 1] = "사용자";
                workSheetForPureOperationMillisecond.Cells[1, 1] = "사용자";
                for (int q = 0; q < 18; q++)
                {
                    workSheetForMovingDistance.Cells[1, q + 2] = $"'{q + 1}";
                    workSheetForWeightMovingDistance.Cells[1, q + 2] = $"'{q + 1}";
                    workSheetForOperationMillisecond.Cells[1, q + 2] = $"'{q + 1}";
                    workSheetForPureOperationMillisecond.Cells[1, q + 2] = $"'{q + 1}";
                }

                // 데이터
                for (int u = 0; u < 22; u++)
                {
                    // 사용자 id
                    workSheetForMovingDistance.Cells[u + 2, 1] = $"{u + 1}";
                    workSheetForWeightMovingDistance.Cells[u + 2, 1] = $"{u + 1}";
                    workSheetForOperationMillisecond.Cells[u + 2, 1] = $"{u + 1}";
                    workSheetForPureOperationMillisecond.Cells[u + 2, 1] = $"{u + 1}";
                    for (int q = 0; q < 18; q++)
                    {
                        UserPathAndTimeItem summation = new UserPathAndTimeItem(0, 0, 0, 0);
                        for (int s = 0; s < 4; s++)
                        {
                            summation += Result.perUser[u, q, s];
                        }

                        workSheetForMovingDistance.Cells[u + 2, q + 2] = summation.MovingDistance;
                        workSheetForWeightMovingDistance.Cells[u + 2, q + 2] = summation.WeightMovingDistance;
                        workSheetForOperationMillisecond.Cells[u + 2, q + 2] = summation.OperationMillisecond;
                        workSheetForPureOperationMillisecond.Cells[u + 2, q + 2] = summation.PureOperationMillisecond;
                    }
                }

                workBook.SaveAs(path, XlFileFormat.xlWorkbookDefault); // 엑셀 파일 저장
                workBook.Close(true);
                excelApp.Quit();
            }
            finally
            {
                Marshal.ReleaseComObject(workSheetForMovingDistance);
                Marshal.ReleaseComObject(workSheetForWeightMovingDistance);
                Marshal.ReleaseComObject(workSheetForOperationMillisecond);
                Marshal.ReleaseComObject(workSheetForPureOperationMillisecond);
                Marshal.ReleaseComObject(workBook);
                Marshal.ReleaseComObject(excelApp);
            }
        }
    }
}
