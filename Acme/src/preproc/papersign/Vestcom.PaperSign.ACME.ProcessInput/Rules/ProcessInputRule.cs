using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Vestcom.PaperSign.ACME.Entities;

namespace Vestcom.PaperSign.ACME.ProcessInput.Rules
{
    /// <summary>
    /// ProcessInputRule class for acme
    /// </summary>
    /// <seealso cref="Vestcom.PaperSign.ACME.ProcessInput.Rules.IBusinessRule" />
    public class ProcessInputRule : IBusinessRule
    {

        public static List<InputFile> InputFileList;
        string packagepath;

        /// <summary>
        /// Execute Business Rules on Input File which is store in DataBase
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="orderNumber"></param>
        /// <param name="inputFilePath"></param>
        public InputFile DownbakExecuteRules(InputFile inputFileRecord, InputRecords records,bool IsKeHeFile=false)
        {

            InputFileList = new List<InputFile>();
            CreateOrdersAndSigns(ref inputFileRecord, records, IsKeHeFile);

            return inputFileRecord;

        }

        /// <summary>
        /// Applies the rules.
        /// </summary>
        /// <param name="lbl">The label.</param>
        /// <param name="clientid">The clientid.</param>
        public void ApplyRules(InputFile lbl, int clientid, string packagePath)
        {
            Common.CheckAccentE(lbl);

            packagepath = packagePath;

            lbl.DEPT_NAME_IN = lbl.DEPT_NAME_IN.EmptyNull().Split(new string[] { "   " }, StringSplitOptions.RemoveEmptyEntries).ElementAtOrDefault(0).EmptyNull().ToUpper();

            if (string.IsNullOrEmpty(lbl.BOGO_TYPE_INFO_IN.EmptyNull().Trim()) && string.IsNullOrEmpty(lbl.SAVINGS_IN.EmptyNull().Trim()))
            {
                if (!string.IsNullOrEmpty(lbl.MFL_SIZE1_IN))
                {
                    lbl.EDLP_SIZE1_IN = lbl.MFL_SIZE1_IN;
                    lbl.MFL_SIZE1_IN = string.Empty;
                }

                if (!string.IsNullOrEmpty(lbl.MFL_SIZE2_IN))
                {
                    lbl.EDLP_SIZE2_IN = lbl.MFL_SIZE2_IN;
                    lbl.MFL_SIZE2_IN = string.Empty;
                }

                if (!string.IsNullOrEmpty(lbl.MFL_SIZE3_IN))
                {
                    lbl.EDLP_SIZE3_IN = lbl.MFL_SIZE3_IN;
                    lbl.MFL_SIZE3_IN = string.Empty;
                }

                if (!string.IsNullOrEmpty(lbl.MFL_SIZE6_IN))
                {
                    lbl.EDLP_SIZE6_IN = lbl.MFL_SIZE6_IN;
                    lbl.MFL_SIZE6_IN = string.Empty;
                }
            }


            if (lbl.DEPT_NAME_IN != lbl.Common.DEPT_NAME_HOLD)
            {
                if (string.IsNullOrEmpty(lbl.DEPT_NAME_IN))
                {
                    lbl.DEPT_NAME_IN = lbl.Common.DEPT_NAME_HOLD ?? string.Empty;
                }
                else
                {
                    lbl.Common.DEPT_NAME_HOLD = lbl.DEPT_NAME_IN;
                }
            }

            if (lbl.FROM_DATE_IN != lbl.Common.FROM_DATE_HOLD)
            {
                if (string.IsNullOrEmpty(lbl.FROM_DATE_IN))
                {
                    lbl.FROM_DATE_IN = lbl.Common.FROM_DATE_HOLD ?? string.Empty;
                }
                else
                {
                    lbl.Common.FROM_DATE_HOLD = lbl.FROM_DATE_IN;
                }
            }

            if (lbl.END_DATE_IN != lbl.Common.END_DATE_HOLD)
            {
                if (string.IsNullOrEmpty(lbl.END_DATE_IN))
                {
                    lbl.END_DATE_IN = lbl.Common.END_DATE_HOLD ?? string.Empty;
                }
                else
                {
                    lbl.Common.END_DATE_HOLD = lbl.END_DATE_IN;
                }
            }

            if (!string.IsNullOrEmpty(lbl.SALE_PR_IN))
            {
                lbl.SALE_PR_IN = lbl.SALE_PR_IN.TrimStart(' ');
            }

            if (!string.IsNullOrEmpty(lbl.UNIT_PR_IN))
            {
                lbl.UNIT_PR_IN = lbl.UNIT_PR_IN.TrimStart(' ');
            }
            else
            {
                lbl.UNIT_PR_IN = lbl.SALE_PR_IN;
            }

            if (!string.IsNullOrEmpty(lbl.SAVINGS_IN))
            {
                lbl.SAVINGS_IN = lbl.SAVINGS_IN.TrimStart(' ');
            }

            lbl.PROD_BRAND_IN = Common.UnQuote(lbl.PROD_BRAND_IN);
            lbl.PROD_BRAND_IN = Common.UpLowCas(lbl.PROD_BRAND_IN);

            if (lbl.PROD_BRAND_IN == "Acme")
            {
                lbl.PROD_BRAND_IN = "ACME";
            }

            lbl.ITEM_DESC_IN = Common.UnQuote(lbl.ITEM_DESC_IN);
            lbl.ITEM_DESC_IN = Common.UpLowCas(lbl.ITEM_DESC_IN);

            lbl.SIZE_IN = Common.UnQuote(lbl.SIZE_IN);
            lbl.SIZE_IN = Common.UpLowCas(lbl.SIZE_IN);

            lbl.SECOND_MSG_IN = Common.UnQuote(lbl.SECOND_MSG_IN);
            lbl.SECOND_MSG_IN = Common.UpLowCas(lbl.SECOND_MSG_IN);

            lbl.UOM_IN = Common.UpLowCas(lbl.UOM_IN);
        }


        /// <summary>
        ///Process input record for Orders and Signs
        /// </summary>
        /// <param name="inputRecord"></param>
        public void CreateOrdersAndSigns(ref InputFile inputRecord, InputRecords records, bool IsKeHeFile=false)
        {

            int Counter = 0;
            InputFile recordToProcess = null;
            InputFile copyOfInputRecord = null;
            //Loop for all the matching promo's
            string WK_PROMO_CODE = "SAL";

            if (IsKeHeFile)
            {
                inputRecord.SL_SIGN_SIZE = "0001";
                inputRecord.SL_SIGN_HEADING = "0001";
                inputRecord.MFL_SIZE = inputRecord.MFL_SIZE2_IN + inputRecord.MFL_SIZE1_IN + inputRecord.MFL_SIZE3_IN + inputRecord.MFL_SIZE6_IN;
                inputRecord.EDLP_SIZE = inputRecord.EDLP_SIZE2_IN + inputRecord.EDLP_SIZE1_IN + inputRecord.EDLP_SIZE3_IN + inputRecord.EDLP_SIZE6_IN;
                ReadHeaderAndSignLayFile(inputRecord, records, Counter);
                CREATE_ORDER_SIGN(inputRecord, records, Counter);
                 // InputFileList.Add(recordToProcess);
                if ( !inputRecord.RECORD_SKIP)
                {
                    CheckforExceptions(inputRecord, records);
                }
            }
            else
            {
                List<PromoPGM> promos = records.PromoPGMs.Where(p => p.PROGRAM_PROMO_CODE.ToUpper() == WK_PROMO_CODE).ToList();
                if (promos != null)
                {
                    copyOfInputRecord = inputRecord.Clone();
                    foreach (PromoPGM promo in promos)
                    {
                        recordToProcess = copyOfInputRecord.Clone();
                        recordToProcess.DEPT_NAME_HOLD = string.Empty;
                        recordToProcess.FROM_DATE_HOLD = string.Empty;
                        recordToProcess.END_DATE_HOLD = string.Empty;

                        //updating the sign size and heading of input record with promo
                       recordToProcess.SL_SIGN_SIZE = promo.PROGRAM_SIGN_SIZE;
                        recordToProcess.SL_SIGN_HEADING = promo.PROGRAM_SIGN_HEAD;
                        
                        // Initializing MFL-SIZE & EDLP-SIZE
                        recordToProcess.MFL_SIZE = recordToProcess.MFL_SIZE2_IN + recordToProcess.MFL_SIZE1_IN + recordToProcess.MFL_SIZE3_IN + recordToProcess.MFL_SIZE6_IN;
                        recordToProcess.EDLP_SIZE = recordToProcess.EDLP_SIZE2_IN + recordToProcess.EDLP_SIZE1_IN + recordToProcess.EDLP_SIZE3_IN + recordToProcess.EDLP_SIZE6_IN;

                        ReadHeaderAndSignLayFile(recordToProcess, records, Counter);

                        if (CREATE_ORDER_SIGN(recordToProcess, records, Counter))
                        {

                            if (Counter > 0)
                            {
                                // Add Multiple Records only if function returns true

                                InputFileList.Add(recordToProcess);

                            }
                            else
                            {
                                // Check upc exception for original record

                                if (!string.IsNullOrEmpty(recordToProcess.UPC_IN))
                                {

                                    inputRecord.UPC_ANALYSIS1 = inputRecord.UPC_IN;
                                }

                                inputRecord = recordToProcess.Clone();
                            }
                            Counter++;

                        }
                        else
                        {
                            ProcessInput.recordNumber--;

                        }


                        if (recordToProcess != null && !recordToProcess.RECORD_SKIP)
                        {
                            CheckforExceptions(recordToProcess, records);
                        }

                        recordToProcess = null; // Reset the input record
                    }

                    if (Counter == 0)
                    {
                        WriteExceptions(inputRecord, "No match for Sign Size/Sign Head in PROMOPGM", "SignSize/SignHead", records);

                    }
                }
            }

        }


        /// <summary>
        ///Process input record to Create Orders and OrderSigns
        /// </summary>
        /// <param name="inputRecord"></param>
        public bool CREATE_ORDER_SIGN(InputFile inputRecord, InputRecords records, int Counter)
        {
            ProcessInput.recordNumber++;
            inputRecord.RowNum = ProcessInput.recordNumber;
            if ((inputRecord.DEPT_NAME_IN.EmptyNull().Trim().ToUpper() == Constants.BAKERY && inputRecord.HEADING_BAKERY_FLAG == Constants.Y) ||
                      (inputRecord.DEPT_NAME_IN.EmptyNull().Trim().ToUpper() == Constants.FLORAL && inputRecord.HEADING_FLORAL_FLAG == Constants.Y) ||
                      (inputRecord.DEPT_NAME_IN.EmptyNull().Trim().ToUpper() == Constants.G_M_AND_H_B_C && inputRecord.HEADING_GM_FLAG == Constants.Y) ||
                      ((inputRecord.DEPT_NAME_IN.ToUpper() == Constants.GROCERY || inputRecord.DEPT_NAME_IN.ToUpper() == Constants.GR) && inputRecord.HEADING_GROCERY_FLAG == Constants.Y) ||
                      (inputRecord.DEPT_NAME_IN.EmptyNull().Trim().ToUpper() == Constants.MEAT_BUTCHER_BLOCK && inputRecord.HEADING_FRESH_MEAT_FLAG == Constants.Y) ||
                      (inputRecord.DEPT_NAME_IN.EmptyNull().Trim().ToUpper() == Constants.MEAT_DELI && inputRecord.HEADING_PKGD_MEAT_FLAG == Constants.Y) ||
                      (inputRecord.DEPT_NAME_IN.EmptyNull().Trim().ToUpper() == Constants.PRODUCE && inputRecord.HEADING_PRODUCE_FLAG == Constants.Y) ||
                      (inputRecord.DEPT_NAME_IN.EmptyNull().Trim().ToUpper() == Constants.SERVICE_DELI && inputRecord.HEADING_SRVC_DELI_FLAG == Constants.Y) ||
                      (inputRecord.DEPT_NAME_IN.EmptyNull().Trim().ToUpper() == Constants.Grocery_KeHe && inputRecord.HEADING_SRVC_DELI_FLAG == Constants.Y) ||
                      (inputRecord.DEPT_NAME_IN.EmptyNull().Trim().ToUpper() == Constants.SEAFOOD && inputRecord.HEADING_SEAFOOD_FLAG == Constants.Y)
                )
            {
                inputRecord.RECORD_SKIP = false;
            }
            else
            {
              
                inputRecord.RECORD_SKIP = true;
                return false;
            }


            if ((!string.IsNullOrEmpty(inputRecord.MFL_SIZE.EmptyNull().Trim()) && inputRecord.HEADING_SMFL_FLAG.ToUpper() == Constants.Y) ||
                !string.IsNullOrEmpty(inputRecord.EDLP_SIZE.EmptyNull().Trim()) && inputRecord.HEADING_EDLP_FLAG.ToUpper() == Constants.Y)
            {
                inputRecord.RECORD_SKIP = false;
            }
            else
            {
            inputRecord.RECORD_SKIP = true;
                return false;
            }



            if ((Constants.ChkList_SL_SIGN_HEADING.Contains(inputRecord.SL_SIGN_HEADING)) &&
                inputRecord.SECOND_MSG_IN.ToUpper() != Constants.Monopoly.ToUpper())
            {
                inputRecord.RECORD_SKIP = true;
                return false;
            }


            if (inputRecord.SECOND_MSG_IN.ToUpper() == Constants.Monopoly.ToUpper() &&
                !(Constants.ChkList_SL_SIGN_HEADING.Contains(inputRecord.SL_SIGN_HEADING)))  // Line no 722
            {
                inputRecord.RECORD_SKIP = true;
                return false;
            }

            if ((inputRecord.HEADING_BOGO_FLAG.ToUpper() == Constants.Y && inputRecord.BOGO_TYPE_INFO_IN.EmptyNull().Trim() != string.Empty && inputRecord.HEADING_SMFL_FLAG.ToUpper() == Constants.Y) ||
                (inputRecord.HEADING_PCNTOFF_FLAG.ToUpper() == Constants.Y && inputRecord.PERCENT_IN.EmptyNull().Trim() != string.Empty && inputRecord.HEADING_SMFL_FLAG.ToUpper() == Constants.Y) ||
                (inputRecord.HEADING_SALE_FLAG.ToUpper() == Constants.Y && inputRecord.BOGO_TYPE_INFO_IN.EmptyNull().Trim() == string.Empty && inputRecord.PERCENT_IN.EmptyNull().Trim() == string.Empty && inputRecord.SALE_PR_IN.EmptyNull().Trim() != string.Empty && inputRecord.HEADING_SMFL_FLAG.ToUpper() == Constants.Y) ||
                (inputRecord.HEADING_EDLP_FLAG.ToUpper() == Constants.Y && inputRecord.SALE_PR_IN.EmptyNull().Trim() != string.Empty))
            {
                inputRecord.RECORD_SKIP = false;
            }
            else
            {
                inputRecord.RECORD_SKIP = true;
                return false;
            }


            if ((inputRecord.HEADING_DUPLEX_FLAG.ToUpper() == Constants.Y && inputRecord.DUPLEX_IN.EmptyNull().Trim().ToUpper() == Constants.Y && Constants.ChkList_Duplex_SL_SIGN_SIZE.Contains(inputRecord.SL_SIGN_SIZE)) ||
                (inputRecord.HEADING_DUPLEX_FLAG.ToUpper() == Constants.N && inputRecord.DUPLEX_IN.EmptyNull().Trim().ToUpper() != Constants.Y) ||
                (inputRecord.HEADING_DUPLEX_FLAG.ToUpper() == Constants.Y && inputRecord.SL_SIGN_SIZE.EmptyNull().Trim().ToUpper() == Constants._0002 && inputRecord.DEPT_NAME_IN.EmptyNull().Trim().ToUpper() == Constants.MEAT_BUTCHER_BLOCK)
                )
            {
                inputRecord.RECORD_SKIP = false;
            }
            else
            {
                inputRecord.RECORD_SKIP = true;
                return false;
            }

            ///////////////Changes received as on 18th Nov 2016
            //In department is floral(dept# = 09), it should only work for SL_SIGN_HEADING 61 & 67
            //For other department it will work for SL_SIGN_HEADING other than 61 & 67
            if ((inputRecord.DEPT_NAME_IN.EmptyNull().Trim().ToUpper() == Constants.FLORAL))
            {
                if (!string.IsNullOrEmpty(inputRecord.SL_SIGN_HEADING) && !(inputRecord.SL_SIGN_HEADING.PadLeft(4, '0').Equals("0061") || inputRecord.SL_SIGN_HEADING.PadLeft(4, '0').Equals("0067") || inputRecord.SL_SIGN_HEADING.PadLeft(4, '0').Equals("0051") || inputRecord.SL_SIGN_HEADING.PadLeft(4, '0').Equals("0053")))
                {
                    inputRecord.RECORD_SKIP = true;
                    return false;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(inputRecord.SL_SIGN_HEADING) && inputRecord.SL_SIGN_HEADING.PadLeft(4, '0').Equals("0061") || inputRecord.SL_SIGN_HEADING.PadLeft(4, '0').Equals("0067"))
                {
                    inputRecord.RECORD_SKIP = true;
                    return false;
                }
            }
         
            if (inputRecord.DEPT_NAME_IN.ToUpper() != inputRecord.DEPT_NAME_HOLD.EmptyNull().ToUpper())
            {
                inputRecord.DEPT_NAME_HOLD = inputRecord.DEPT_NAME_IN;

                CreateOrder(inputRecord, records, Counter); // Function call
            }


            inputRecord.OS_QUANTITY_NUMERIC = 0;

            //SIZE 1
            if (inputRecord.SL_SIGN_SIZE.PadLeft(4, '0') == Constants._0001)
            {
                if (inputRecord.HEADING_SMFL_FLAG.ToUpper() == Constants.Y)
                {
                    if (!string.IsNullOrEmpty(inputRecord.MFL_SIZE1_IN))
                    {
                        inputRecord.OS_QUANTITY_NUMERIC = inputRecord.MFL_SIZE1_IN.EmptyNull().Trim().Split(' ').ElementAtOrDefault(0).ConvertStringToInteger();
                    }
                }
                else
                {
                    if (inputRecord.HEADING_EDLP_FLAG.ToUpper() == Constants.Y && !string.IsNullOrEmpty(inputRecord.EDLP_SIZE1_IN))
                    {
                       
                            inputRecord.OS_QUANTITY_NUMERIC = inputRecord.EDLP_SIZE1_IN.EmptyNull().Trim().Split(' ').ElementAtOrDefault(0).ConvertStringToInteger();
                     
                    }
                }
            }
            //SIZE 2
            if (inputRecord.SL_SIGN_SIZE.PadLeft(4, '0') == Constants._0002)
            {
                if (inputRecord.HEADING_SMFL_FLAG.ToUpper() == Constants.Y)
                {
                    if (!string.IsNullOrEmpty(inputRecord.MFL_SIZE2_IN.EmptyNull().Trim()))
                    {
                        inputRecord.OS_QUANTITY_NUMERIC = inputRecord.MFL_SIZE2_IN.EmptyNull().Trim().Split(' ').ElementAtOrDefault(0).ConvertStringToInteger();
                    }
                }
                else
                {
                    if (inputRecord.HEADING_EDLP_FLAG.ToUpper() == Constants.Y && !string.IsNullOrEmpty(inputRecord.EDLP_SIZE2_IN.EmptyNull().Trim()))
                    {                       
                       inputRecord.OS_QUANTITY_NUMERIC = inputRecord.EDLP_SIZE2_IN.EmptyNull().Trim().Split(' ').ElementAtOrDefault(0).ConvertStringToInteger();
                    }
                }
            }

            //SIZE 3
            if (inputRecord.SL_SIGN_SIZE.PadLeft(4, '0') == Constants._0003)
            {
                if (inputRecord.HEADING_SMFL_FLAG.ToUpper() == Constants.Y)
                {
                    if (!string.IsNullOrEmpty(inputRecord.MFL_SIZE3_IN.EmptyNull().Trim()))
                    {
                        inputRecord.OS_QUANTITY_NUMERIC = inputRecord.MFL_SIZE3_IN.EmptyNull().Trim().Split(' ').ElementAtOrDefault(0).ConvertStringToInteger();
                    }
                }
                else
                {
                    if (inputRecord.HEADING_EDLP_FLAG.ToUpper() == Constants.Y && !string.IsNullOrEmpty(inputRecord.EDLP_SIZE3_IN.EmptyNull().Trim()))
                    {
                         inputRecord.OS_QUANTITY_NUMERIC = inputRecord.EDLP_SIZE3_IN.EmptyNull().Trim().Split(' ').ElementAtOrDefault(0).ConvertStringToInteger();
                    }
                }
            }

            //SIZE 6
            if (inputRecord.SL_SIGN_SIZE.PadLeft(4, '0') == Constants._0006)
            {
                if (inputRecord.HEADING_SMFL_FLAG.ToUpper() == Constants.Y)
                {
                    if (!string.IsNullOrEmpty(inputRecord.MFL_SIZE6_IN.EmptyNull().Trim()))
                    {
                        inputRecord.OS_QUANTITY_NUMERIC = inputRecord.MFL_SIZE6_IN.EmptyNull().Trim().Split(' ').ElementAtOrDefault(0).ConvertStringToInteger();
                    }
                }
                else
                {
                    if (inputRecord.HEADING_EDLP_FLAG.ToUpper() == Constants.Y && !string.IsNullOrEmpty(inputRecord.EDLP_SIZE6_IN.EmptyNull().Trim()))
                    {
                            inputRecord.OS_QUANTITY_NUMERIC = inputRecord.EDLP_SIZE6_IN.EmptyNull().Trim().Split(' ').ElementAtOrDefault(0).ConvertStringToInteger();
                    }
                }
            }

            inputRecord.OS_QUANTITY = inputRecord.OS_QUANTITY_NUMERIC;

            inputRecord.WORK_DATE = string.Empty;

            if (inputRecord.END_DATE_IN != null && inputRecord.END_DATE_IN.Contains('/'))
            {
                inputRecord.WORK_DATE_MONTH = inputRecord.END_DATE_IN.EmptyNull().Split('/').ElementAtOrDefault(0);
                inputRecord.WORK_DATE_DAY = inputRecord.END_DATE_IN.EmptyNull().Split('/').ElementAtOrDefault(1);
                inputRecord.WORK_DATE_YEAR = inputRecord.END_DATE_IN.EmptyNull().Split('/').ElementAtOrDefault(2).SafeSubstring(0, 4);

            }
            else if (inputRecord.END_DATE_IN != null && inputRecord.END_DATE_IN.Contains(' '))
            {
                inputRecord.WORK_DATE_MONTH = inputRecord.END_DATE_IN.EmptyNull().Split(' ').ElementAtOrDefault(0);
                inputRecord.WORK_DATE_DAY = inputRecord.END_DATE_IN.EmptyNull().Split(' ').ElementAtOrDefault(1);
                inputRecord.WORK_DATE_YEAR = inputRecord.END_DATE_IN.EmptyNull().Split(' ').ElementAtOrDefault(2).SafeSubstring(0, 4);
            }

            inputRecord.WORK_DATE = inputRecord.WORK_DATE_YEAR + inputRecord.WORK_DATE_MONTH + inputRecord.WORK_DATE_DAY;

            if (!string.IsNullOrEmpty(inputRecord.BOARS_HEAD_DAT.EmptyNull().Trim()))
            {
                inputRecord.RESTRICT_CODE = Constants.AA;
            }

            if (!string.IsNullOrEmpty(inputRecord.LIMIT_IN.EmptyNull().Trim()))
            {
                inputRecord.LIMIT_COUNT = 0;

            }

            if (!string.IsNullOrEmpty(inputRecord.MUST_BUY_IN.EmptyNull().Trim())) // line no 948
            {
                inputRecord.MUST_BUY_IN = inputRecord.MUST_BUY_IN.EmptyNull().Replace('$', ' ');
                inputRecord.MUST_BUY_IN = inputRecord.MUST_BUY_IN.EmptyNull().Replace(".00", "   ");

            }

            if (inputRecord.HEADING_BOGO_FLAG == Constants.Y && inputRecord.BOGO_TYPE_INFO_IN.EmptyNull().Trim() != string.Empty)
            {

                if (inputRecord.BOGO_TYPE_INFO_IN.ToUpper() == Constants.BOGO)
                {

                    inputRecord.BOGO_BUY_COUNT_DISPLAY = 2;
                }
                else
                {
                    inputRecord.BOGO_WORK_AREA = string.Empty;
                    inputRecord.BOGO_BUY_COUNT = 0;
                    inputRecord.BOGO_GET_COUNT = 0;

                    inputRecord.DUMMY = inputRecord.BOGO_TYPE_INFO_IN.EmptyNull().Split(new string[] { "B", "G", " " }, StringSplitOptions.None).ElementAtOrDefault(0);

                    inputRecord.BOGO_BUY_COUNT = inputRecord.BOGO_TYPE_INFO_IN.EmptyNull().Split(new string[] { "B", "G", " " }, StringSplitOptions.None).ElementAtOrDefault(1).ConvertStringToInteger();
                    inputRecord.BOGO_GET_COUNT = inputRecord.BOGO_TYPE_INFO_IN.EmptyNull().Split(new string[] { "B", "G", " " }, StringSplitOptions.None).ElementAtOrDefault(2).ConvertStringToInteger();

                    inputRecord.BOGO_BUY_COUNT_DISPLAY = inputRecord.BOGO_BUY_COUNT;
                    inputRecord.BOGO_GET_COUNT_DISPLAY = inputRecord.BOGO_GET_COUNT;


                    inputRecord.BOGO_BUY_COUNT = inputRecord.BOGO_BUY_COUNT + inputRecord.BOGO_GET_COUNT;
                    inputRecord.BOGO_GET_COUNT_DISPLAY = inputRecord.BOGO_GET_COUNT;
                }

                inputRecord.PRICE_IN = inputRecord.SALE_PR_IN;
                ProcessPrice(inputRecord, records); // function call


                if (inputRecord.SALE_MULTI_IN == 0)
                {
                    inputRecord.SALE_MULTI_IN = 1;
                }

                inputRecord.OS_SPECIAL_FOR_NUMERIC = inputRecord.SALE_MULTI_IN;


                inputRecord.PRICE_IN = inputRecord.SAVINGS_IN;

                ProcessPrice(inputRecord, records); // function call

                ComputeUnitPrice(inputRecord, records); // function call
            }
            
                if (inputRecord.HEADING_SALE_FLAG == Constants.Y&&inputRecord.BOGO_TYPE_INFO_IN != null && inputRecord.BOGO_TYPE_INFO_IN.EmptyNull().Trim() == string.Empty && inputRecord.PERCENT_IN != null && inputRecord.PERCENT_IN.EmptyNull().Trim() == string.Empty)
                {
                    inputRecord.PRICE_IN = inputRecord.SALE_PR_IN;

                    ProcessPrice(inputRecord, records); // function call
                    {
                        if (inputRecord.SALE_MULTI_IN == 0)
                        {
                            inputRecord.SALE_MULTI_IN = 1;
                        }
                        inputRecord.OS_SPECIAL_FOR_NUMERIC = inputRecord.SALE_MULTI_IN;


                        if (!string.IsNullOrEmpty(inputRecord.MUST_BUY_IN))
                        {
                        inputRecord.MUST_BUY_IN = "";
                        }
                        else
                        {
                            inputRecord.SALE_MULTI_DISPLAY = inputRecord.SALE_MULTI_IN;

                        }

                        inputRecord.PRICE_IN = inputRecord.SAVINGS_IN;
                        ProcessPrice(inputRecord, records); // Function call

                        inputRecord.WORK_SAVINGS = inputRecord.WORK_PRICE;


                    }
                    ComputeUnitPrice(inputRecord, records); // function call
                }
          
            if (inputRecord.HEADING_PCNTOFF_FLAG == Constants.Y && !string.IsNullOrEmpty(inputRecord.PERCENT_IN))
            {
              

                    inputRecord.PRICE_IN = inputRecord.SALE_PR_IN;
                    ProcessPrice(inputRecord, records); // function call                                                        
                  
                        if (inputRecord.SALE_MULTI_IN == 0)
                        {
                            inputRecord.SALE_MULTI_IN = 1;
                        }
                        inputRecord.OS_SPECIAL_FOR_NUMERIC = inputRecord.SALE_MULTI_IN;


                        if (!string.IsNullOrEmpty(inputRecord.MUST_BUY_IN))
                        {
                        inputRecord.MUST_BUY_IN = string.Empty;
                        }
                        else
                        {
                            inputRecord.SALE_MULTI_DISPLAY = inputRecord.SALE_MULTI_IN;

                        }
                        inputRecord.PRICE_IN = inputRecord.SAVINGS_IN;
                        ProcessPrice(inputRecord, records); // Function call

                        inputRecord.WORK_SAVINGS = inputRecord.WORK_PRICE;
                    
                    ComputeUnitPrice(inputRecord, records); // function call
               
            }


            if (inputRecord.HEADING_EDLP_FLAG == Constants.Y)
            {
                if (inputRecord.SALE_MULTI_IN == 0)
                {
                    inputRecord.SALE_MULTI_IN = 1;
                }
                inputRecord.OS_SPECIAL_FOR_NUMERIC = inputRecord.SALE_MULTI_IN;
                inputRecord.PRICE_IN = inputRecord.SAVINGS_IN;
                ProcessPrice(inputRecord, records); // Function call
                ComputeUnitPrice(inputRecord, records); // function call
            }

            if (!string.IsNullOrEmpty(inputRecord.UPC_IN))
            {

                inputRecord.UPC_ANALYSIS1 = inputRecord.UPC_IN;
                ProcessUpc(inputRecord, records); // function call
            }
           
            return true;


        }


        /// <summary>
        /// Checkfors the exceptions.
        /// </summary>
        /// <param name="lbl">The label.</param>
        /// <param name="records">The records.</param>
        private void CheckforExceptions(InputFile lbl, InputRecords records)
        {
            SubstituteStock record;
            record = records.SubstituteStock.FirstOrDefault(s => s.MAIN_STOCK == lbl.SL_PAPER_TYPE);
          
                if (record != null&&string.IsNullOrEmpty(lbl.D_BACKGROUND_IMAGE) && !string.IsNullOrEmpty(record.BG_IMG))
                    lbl.D_BACKGROUND_IMAGE = Path.Combine(Constants.IMAGEFOLDERPATH, (record.BG_IMG + Constants.IMAGEFILEEXTENSION));

            if (!String.IsNullOrEmpty(lbl.D_BACKGROUND_IMAGE) && !File.Exists(lbl.D_BACKGROUND_IMAGE))
            {
                WriteExceptions(lbl, "ART IS MISSING!!! FOR UPC:" + lbl.UPC_IN + " AND IMAGE NAME:" + record.BG_IMG + Constants.IMAGEFILEEXTENSION + " PLEASE CREATE AND ADD TO IMAGE LIBRARY ASAP.", "", records);
            }

            if (String.IsNullOrEmpty(lbl.SL_SIGN_SIZE) || String.IsNullOrEmpty(lbl.SL_SIGN_HEADING))
            {
                WriteExceptions(lbl, "Sign Size/Sign Head not present.", "SignSize/SignHead", records);
            }
            if (!string.IsNullOrEmpty(lbl.UPC_IN))
            {
                long upc;
                if (!long.TryParse(lbl.UPC_IN, out upc))
                {
                    WriteExceptions(lbl, "Incorrect Upc format.Upc value not numeric.", "Upc", records);
                }
               
            }
        }

        /// <summary>
        ///Process the UPC Code
        /// </summary>
        /// <param name="inputRecord"></param>
        public void ProcessUpc(InputFile inputRecord, InputRecords records)
        {


            //all the variables are substring of UPC_ANALYSIS1 , are to be intialised
            inputRecord.UPC_ANAL1_PART1 = Helper.SafeSubstring(inputRecord.UPC_ANALYSIS1, 0, 5);
            inputRecord.UPC_ANAL1_PART2 = Helper.SafeSubstring(inputRecord.UPC_ANALYSIS1, 5, 5);
            inputRecord.UPC_ANAL1_PART3 = Helper.SafeSubstring(inputRecord.UPC_ANALYSIS1, 10, 3);

            inputRecord.UPC_ANAL2_PART1 = Helper.SafeSubstring(inputRecord.UPC_ANALYSIS1, 0, 5);
            inputRecord.UPC_ANAL2_MIDDLE = Helper.SafeSubstring(inputRecord.UPC_ANALYSIS1, 5, 1);
            inputRecord.UPC_ANAL2_PART2 = Helper.SafeSubstring(inputRecord.UPC_ANALYSIS1, 6, 5);
            inputRecord.UPC_ANAL2_PART3 = Helper.SafeSubstring(inputRecord.UPC_ANALYSIS1, 11, 2);


            inputRecord.UPC_ANAL3_PART1 = Helper.SafeSubstring(inputRecord.UPC_ANALYSIS1, 0, 6);
            inputRecord.UPC_ANAL3_PART2 = Helper.SafeSubstring(inputRecord.UPC_ANALYSIS1, 6, 5);
            inputRecord.UPC_ANAL3_PART3 = Helper.SafeSubstring(inputRecord.UPC_ANALYSIS1, 11, 2);

            inputRecord.UPC_ANAL4_PART1 = Helper.SafeSubstring(inputRecord.UPC_ANALYSIS1, 0, 1);
            inputRecord.UPC_ANAL4_DASH1 = Helper.SafeSubstring(inputRecord.UPC_ANALYSIS1, 1, 1);
            inputRecord.UPC_ANAL4_PART2 = Helper.SafeSubstring(inputRecord.UPC_ANALYSIS1, 2, 5);
            inputRecord.UPC_ANAL4_DASH2 = Helper.SafeSubstring(inputRecord.UPC_ANALYSIS1, 7, 1);
            inputRecord.UPC_ANAL4_PART3 = Helper.SafeSubstring(inputRecord.UPC_ANALYSIS1, 8, 5);

            inputRecord.UPC_ANAL5_PART1 = Helper.SafeSubstring(inputRecord.UPC_ANALYSIS1, 0, 4);
            inputRecord.UPC_ANAL5_PART2 = Helper.SafeSubstring(inputRecord.UPC_ANALYSIS1, 4, 9);

            inputRecord.UPC_ANAL6_PART1 = Helper.SafeSubstring(inputRecord.UPC_ANALYSIS1, 0, 6);
            inputRecord.UPC_ANAL6_PART2 = Helper.SafeSubstring(inputRecord.UPC_ANALYSIS1, 6, 1);
            inputRecord.UPC_ANAL6_PART3 = Helper.SafeSubstring(inputRecord.UPC_ANALYSIS1, 7, 5);
            inputRecord.UPC_ANAL6_PART4 = Helper.SafeSubstring(inputRecord.UPC_ANALYSIS1, 12, 1);


            inputRecord.UPC_ANAL7_PART1 = Helper.SafeSubstring(inputRecord.UPC_ANALYSIS1, 0, 6);
            inputRecord.UPC_ANAL7_PART2 = Helper.SafeSubstring(inputRecord.UPC_ANALYSIS1, 6, 1);
            inputRecord.UPC_ANAL7_PART3 = Helper.SafeSubstring(inputRecord.UPC_ANALYSIS1, 7, 5);
            inputRecord.UPC_ANAL7_PART4 = Helper.SafeSubstring(inputRecord.UPC_ANALYSIS1, 12, 1);

            inputRecord.UPC_ANAL8_PART1 = Helper.SafeSubstring(inputRecord.UPC_ANALYSIS1, 0, 11);
            inputRecord.UPC_ANAL8_PART2 = Helper.SafeSubstring(inputRecord.UPC_ANALYSIS1, 11, 2);

        }

        /// <summary>
        ///Process the work Price
        /// </summary>
        /// <param name="inputRecord"></param>
        public void ProcessPrice(InputFile inputRecord, InputRecords records)
        {

            inputRecord.WORK_PRICE = "0000000";
            inputRecord.PRICE_IN = inputRecord.PRICE_IN.EmptyNull().Replace('$', '0');
            inputRecord.PRICE_IN = Regex.Replace(inputRecord.PRICE_IN, @"^\s+|", match => match.Value.Replace(' ', '0'));

            if (!string.IsNullOrEmpty(inputRecord.PRICE_IN) && inputRecord.PRICE_IN.Contains('.'))
            {
                inputRecord.WORK_DOLLARS = inputRecord.PRICE_IN.EmptyNull().Split('.').ElementAtOrDefault(0);
                inputRecord.WORK_CENTS = inputRecord.PRICE_IN.EmptyNull().Split('.').ElementAtOrDefault(1);
            }
            else if (!string.IsNullOrEmpty(inputRecord.PRICE_IN) && inputRecord.PRICE_IN.Contains(' '))
            {
                inputRecord.WORK_DOLLARS = inputRecord.PRICE_IN.EmptyNull().Split(' ').ElementAtOrDefault(0);
                inputRecord.WORK_CENTS = inputRecord.PRICE_IN.EmptyNull().Split(' ').ElementAtOrDefault(1);
            }

            inputRecord.WORK_PRICE = inputRecord.WORK_DOLLARS + inputRecord.WORK_CENTS;

        }

        /// <summary>
        ///Process the OS-PackageSize & OS-Unit Price
        /// </summary>
        /// <param name="inputRecord"></param>
        public void ComputeUnitPrice(InputFile inputRecord, InputRecords records)
        {

            inputRecord.UP_FIELD1 = string.Empty;
            inputRecord.UP_FIELD2 = string.Empty;
            inputRecord.UP_FIELD3 = string.Empty;
            inputRecord.UP_FIELD4 = string.Empty;


            inputRecord.UP_FIELD1 = inputRecord.UOM_IN.EmptyNull().Split(new string[] { " ", "/", "|" }, StringSplitOptions.None).ElementAtOrDefault(0);
            inputRecord.UP_FIELD2 = inputRecord.UOM_IN.EmptyNull().Split(new string[] { " ", "/", "|" }, StringSplitOptions.None).ElementAtOrDefault(1);
            inputRecord.UP_FIELD3 = inputRecord.UOM_IN.EmptyNull().Split(new string[] { " ", "/", "|" }, StringSplitOptions.None).ElementAtOrDefault(2);
            inputRecord.UP_FIELD4 = inputRecord.UOM_IN.EmptyNull().Split(new string[] { " ", "/", "|" }, StringSplitOptions.None).ElementAtOrDefault(3);

            inputRecord.UP_FIELD1 = string.IsNullOrEmpty(inputRecord.UP_FIELD1) ? string.Empty : inputRecord.UP_FIELD1;
            inputRecord.UP_FIELD2 = string.IsNullOrEmpty(inputRecord.UP_FIELD2) ? string.Empty : inputRecord.UP_FIELD2;
            inputRecord.UP_FIELD3 = string.IsNullOrEmpty(inputRecord.UP_FIELD3) ? string.Empty : inputRecord.UP_FIELD3;
            inputRecord.UP_FIELD4 = string.IsNullOrEmpty(inputRecord.UP_FIELD4) ? string.Empty : inputRecord.UP_FIELD4;


            if (string.IsNullOrEmpty(inputRecord.UP_FIELD2))
            {

                return;
            }


            if (!string.IsNullOrEmpty(inputRecord.UP_FIELD1) && !string.IsNullOrEmpty(inputRecord.UP_FIELD2))
            {
                inputRecord.DASH_COUNT = 0;
                inputRecord.DASH_COUNT = inputRecord.UP_FIELD1.EmptyNull().Trim().Count(f => f == '-');
                if (inputRecord.DASH_COUNT > 0)
                {
                    inputRecord.WORK_FIELD = string.Empty;
                    inputRecord.WORK_FIELD = inputRecord.UP_FIELD1.EmptyNull().Split('-').ElementAtOrDefault(0);
                    inputRecord.UP_FIELD1 = inputRecord.WORK_FIELD;
                }

                inputRecord.UP_CODE_IN = inputRecord.UP_FIELD2;
                inputRecord.UP_TYPE = string.Empty;

                switch (inputRecord.UP_CODE_IN)
                {
                    case "Lb":
                    case "Oz":
                        inputRecord.UP_TYPE = ".d";
                        break;

                    case "Ct":
                        inputRecord.UP_TYPE = ".c";
                        break;

                    case "Ea":
                        inputRecord.UP_TYPE = ".e";
                        break;

                    case "Fl":
                    case "Lt":
                        inputRecord.UP_TYPE = ".l";
                        break;

                    case "Sq":
                        inputRecord.UP_TYPE = ".a";
                        break;

                    case "Ft":
                    case "Fo":
                    case "Yd":
                        inputRecord.UP_TYPE = ".f";
                        break;

                    case "Pt":
                        inputRecord.UP_TYPE = ".p";
                        break;
                }

                UnitPric_Main(inputRecord, records); // function call to UNITPRIC.CBL main method
            }

        }

        /// <summary>
        ///Process order for input record
        /// </summary>
        /// <param name="inputRecord"></param>
        public void CreateOrder(InputFile inputRecord, InputRecords records, int Counter)
        {

            inputRecord.WORK_WH_NUMBER = "01";

            if (string.IsNullOrEmpty(inputRecord.DEPT_NAME_IN))
            {
                inputRecord.DEPT_NAME_IN = string.Empty;
            }


            Department dept = records.Departments.FirstOrDefault(dep => dep.DepartmentName.ToUpper() == inputRecord.DEPT_NAME_IN.ToUpper());

            if (dept != null)
            {
                inputRecord.O_DEPARTMENT_NO = dept.DepartmentNumber;
                inputRecord.IMAGE_FILE_SIGN_SIZE = inputRecord.SL_SIGN_SIZE;


                inputRecord.IMAGE_FILE_SIGN_HEADING = inputRecord.SL_SIGN_HEADING;

            }
            else
            {
                inputRecord.MSG_MESSAGE = string.Empty;
                inputRecord.MSG_MESSAGE = "Department " + inputRecord.DEPT_NAME_IN.EmptyNull().Split(new string[] { "  " }, StringSplitOptions.None).ElementAtOrDefault(0) + " not recognized. Skipping image processing.";
                inputRecord.MSG_CR = Constants.Y;
                Common.CALL_MSGDRVR(inputRecord, packagepath);
                WriteExceptions(inputRecord, inputRecord.MSG_MESSAGE, inputRecord.DEPT_NAME_IN, records);


            }

            // below set of statement's are for CREATE-ORDER-PART3
            inputRecord.SIGN_NUMBER = 0;

            if (!string.IsNullOrEmpty(inputRecord.O_DEPARTMENT_NO) && inputRecord.O_DEPARTMENT_NO.PadLeft(2, '0') == "03" && !string.IsNullOrEmpty(inputRecord.SL_SIGN_SIZE) && inputRecord.SL_SIGN_SIZE.PadLeft(4, '0') == "0002")
            {
                inputRecord.SL_SIGN_SIZE = inputRecord.HOLD_SL_SIGN_SIZE;
                inputRecord.SL_SIGN_HEADING = inputRecord.HOLD_SL_SIGN_HEADING;
                inputRecord.SL_PAPER_TYPE = inputRecord.HOLD_SL_PAPER_TYPE;
                inputRecord.SL_LAYOUT_NO = inputRecord.HOLD_SL_LAYOUT_NO;
            }

        }

        /// <summary>
        ///Reads Header and Signlayout file
        /// </summary>
        /// <param name="inputRecord"></param>
        public void ReadHeaderAndSignLayFile(InputFile inputRecord, InputRecords records, int Counter)
        {


            if (string.IsNullOrEmpty(inputRecord.SL_SIGN_HEADING))
            {
                inputRecord.SL_SIGN_HEADING = string.Empty;
            }

            Heading heading = records.Headings.FirstOrDefault(h => h.HEADING_NO.PadLeft(4, '0') == inputRecord.SL_SIGN_HEADING.PadLeft(4, '0'));
            if (heading != null)
            {
                inputRecord.HEADING_SALE_FLAG = heading.HEADING_SALE_FLAG;
                inputRecord.HEADING_BOGO_FLAG = heading.HEADING_BOGO_FLAG;
                inputRecord.HEADING_PCNTOFF_FLAG = heading.HEADING_PCNTOFF_FLAG;
                inputRecord.HEADING_10FOR10_FLAG = heading.HEADING_10FOR10_FLAG;
                inputRecord.HEADING_GROCERY_FLAG = heading.HEADING_GROCERY_FLAG;
                inputRecord.HEADING_BAKERY_FLAG = heading.HEADING_BAKERY_FLAG;
                inputRecord.HEADING_FRESH_MEAT_FLAG = heading.HEADING_FRESH_MEAT_FLAG;
                inputRecord.HEADING_SRVC_DELI_FLAG = heading.HEADING_SRVC_DELI_FLAG;
                inputRecord.HEADING_PKGD_MEAT_FLAG = heading.HEADING_PKGD_MEAT_FLAG;
                inputRecord.HEADING_PRODUCE_FLAG = heading.HEADING_PRODUCE_FLAG;
                inputRecord.HEADING_FLORAL_FLAG = heading.HEADING_FLORAL_FLAG;
                inputRecord.HEADING_GM_FLAG = heading.HEADING_GM_FLAG;
                inputRecord.HEADING_BEER_WINE_FLAG = heading.HEADING_BEER_WINE_FLAG;
                inputRecord.HEADING_FROZEN_FLAG = heading.HEADING_FROZEN_FLAG;
                inputRecord.HEADING_SEAFOOD_FLAG = heading.HEADING_SEAFOOD_FLAG;
                inputRecord.HEADING_SMFL_FLAG = heading.HEADING_SMFL_FLAG;
                inputRecord.HEADING_EDLP_FLAG = heading.HEADING_EDLP_FLAG;
                inputRecord.HEADING_OTHER_FLAG = heading.HEADING_OTHER_FLAG;
                inputRecord.HEADING_DUPLEX_FLAG = heading.HEADING_DUPLEX_FLAG;
            }
            else
            {
                inputRecord.MSG_MESSAGE = string.Empty;
                inputRecord.MSG_MESSAGE = "Heading " + inputRecord.SL_SIGN_HEADING + " is not in the Heading table . . . skipping.";
                WriteExceptions(inputRecord, inputRecord.MSG_MESSAGE, "SL_SIGN_HEADING", records);
                return;
            }

            SignLayout signLay = records.SignLayouts.FirstOrDefault(s => s.SL_SIGN_SIZE.PadLeft(4, '0') == inputRecord.SL_SIGN_SIZE.PadLeft(4, '0') && s.SL_SIGN_HEADING.PadLeft(4, '0') == inputRecord.SL_SIGN_HEADING.PadLeft(4, '0'));
            if (signLay != null)
            {
                inputRecord.SL_LAYOUT_NO = signLay.SL_LAYOUT_NO;
                inputRecord.SL_PAPER_TYPE = signLay.SL_PAPER_TYPE;
                inputRecord.SignSizeId = signLay.SignSizeId;
                inputRecord.SignHeaderId = signLay.SignHeaderId;

                inputRecord.HOLD_SL_SIGN_SIZE = inputRecord.SL_SIGN_SIZE;
                inputRecord.HOLD_SL_SIGN_HEADING = inputRecord.SL_SIGN_HEADING;
                inputRecord.HOLD_SL_PAPER_TYPE = inputRecord.SL_PAPER_TYPE;
                inputRecord.HOLD_SL_LAYOUT_NO = inputRecord.SL_LAYOUT_NO;
            }
            else
            {
                inputRecord.MSG_MESSAGE = string.Empty;
                inputRecord.MSG_MESSAGE = "No Layout-Stock record for Size = " + inputRecord.SL_SIGN_SIZE + ", Heading = " + inputRecord.SL_SIGN_HEADING + " - stopping.";
                WriteExceptions(inputRecord, inputRecord.MSG_MESSAGE, "SL_SIGN_SIZE", records);
            }

        }

        /// <summary>
        ///Process the work Price as per UNITPRIC.cbl
        /// </summary>
        /// <param name="inputRecord"></param>
        public void UnitPric_ComputeUnitPrice(InputFile inputRecord, InputRecords records)
        {

            inputRecord.WORK_UNITS_PART1 = "0";
            inputRecord.PACK_QTY = "0";
            inputRecord.WORK_UNITS_PART2 = "0";
            inputRecord.DECIMAL_POINT_COUNT  = 0;
            inputRecord.ASTERISK_COUNT = 0;
            if ((!string.IsNullOrEmpty(inputRecord.PS_SIZE)) && inputRecord.PS_SIZE.ToUpper() == "HALF")
            {
                inputRecord.PS_SIZE = ".5";
            }
            else if (((!string.IsNullOrEmpty(inputRecord.PS_SIZE)) && inputRecord.PS_SIZE.ToUpper() == "PER") || ((!string.IsNullOrEmpty(inputRecord.PS_SIZE)) && inputRecord.PS_SIZE.ToUpper() == "EACH"))
            {
                inputRecord.PS_SIZE = "1.0";
            }
           

            inputRecord.ASTERISK_COUNT = inputRecord.PS_SIZE.EmptyNull().Trim().Count(f => f == '*');

            if (inputRecord.ASTERISK_COUNT == 1)
            {
                inputRecord.TEMP_SIZE = string.Empty;
                inputRecord.PACK_QTY = inputRecord.PS_SIZE.EmptyNull().Split(new string[] { "*", "  " }, StringSplitOptions.None).ElementAtOrDefault(0);
                inputRecord.TEMP_SIZE = inputRecord.PS_SIZE.EmptyNull().Split(new string[] { "*", "  " }, StringSplitOptions.None).ElementAtOrDefault(1);
                inputRecord.PS_SIZE = inputRecord.TEMP_SIZE;
            }
            else
            {
                inputRecord.PACK_QTY = "1";
            }

            inputRecord.DECIMAL_POINT_COUNT = inputRecord.PS_SIZE.EmptyNull().Trim().Count(f => f == '.');

            if (inputRecord.DECIMAL_POINT_COUNT == 0)
            {
                inputRecord.WORK_UNITS_PART1 = inputRecord.PS_SIZE.EmptyNull().Split(' ').ElementAtOrDefault(0);
            }
            else
            {
                inputRecord.WORK_UNITS_PART2_COUNT = 0;
                inputRecord.WORK_UNITS_PART1 = inputRecord.PS_SIZE.EmptyNull().Split(new string[] { ".", " " }, StringSplitOptions.None).ElementAtOrDefault(0);
                inputRecord.WORK_UNITS_PART2 = inputRecord.PS_SIZE.EmptyNull().Split(new string[] { ".", " " }, StringSplitOptions.None).ElementAtOrDefault(1);

                inputRecord.WORK_UNITS_PART2_COUNT = inputRecord.WORK_UNITS_PART2.Length;

                if (inputRecord.WORK_UNITS_PART2_COUNT == 1)
                {
                    inputRecord.WORK_UNITS_PART2 = inputRecord.WORK_UNITS_PART2 + "0";
                }
            }


            inputRecord.WORK_UNITS = inputRecord.WORK_UNITS_PART1.PadLeft(5, '0') + inputRecord.WORK_UNITS_PART2.PadLeft(2, '0');

            inputRecord.WORK_UNITS_COMPUTATIONAL = inputRecord.WORK_UNITS;


            inputRecord.WORK_UNITS_COMPUTATIONAL = (inputRecord.WORK_UNITS_COMPUTATIONAL.ConvertStringToDouble() * inputRecord.PACK_QTY.ConvertStringToDouble()).ToString();
            
            if (inputRecord.OS_SPECIAL_FOR_NUMERIC > 0)
            {
                inputRecord.SPECIAL_PRICE_COMPUTATIONAL = (inputRecord.SPECIAL_PRICE_COMPUTATIONAL.ConvertStringToDouble() / inputRecord.OS_SPECIAL_FOR_NUMERIC).ToString();
            }
            else
            {
                inputRecord.SPECIAL_PRICE_COMPUTATIONAL = "0.0";
            }

            if (inputRecord.WORK_UNITS_COMPUTATIONAL.ConvertStringToDouble() > 0)
            {
                inputRecord.UNIT_PRICE_COMPUTATIONAL = (inputRecord.SPECIAL_PRICE_COMPUTATIONAL.ConvertStringToDouble() / inputRecord.WORK_UNITS_COMPUTATIONAL.ConvertStringToDouble()).ToString();
            }
            else
            {
                inputRecord.UNIT_PRICE_COMPUTATIONAL = "0.0";
            }



            do
            {
                inputRecord.RE_SEARCH_FLAG = Constants.N;
                inputRecord.PS_KEY = inputRecord.PS_TYPE_CHAR2.PadRight(1, ' ') + inputRecord.PS_UNITS.PadRight(10, ' ') + inputRecord.PS_UNITS2.PadRight(10, ' ');
                UnitPriceEntry upe = records.UnitPriceEntrys.FirstOrDefault(u => u.UPE_KEY == inputRecord.PS_KEY);
                if (upe != null)
                {
                    inputRecord.UPE_UNIT = upe.UPE_UNIT;
                    inputRecord.UPE_NUMERATOR = upe.UPE_NUMERATOR;
                    inputRecord.UPE_DENOMINATOR = upe.UPE_DENOMINATOR;
                    AdjustUnitPrice(inputRecord, records);   // Function Call
                }
                else
                {
                    RejectUnitPriceUnits(inputRecord, records);

                }
            } while (inputRecord.RE_SEARCH_FLAG == Constants.Y);


            inputRecord.WK_UNIT_PRICE = string.Empty;

            if (inputRecord.UNIT_PRICE_COMPUTATIONAL.ConvertStringToDouble() > .99)
            {
                inputRecord.UNIT_PRICE_COMPUTATIONAL = (inputRecord.UNIT_PRICE_COMPUTATIONAL.ConvertStringToDouble() + .0099).ToString();
                inputRecord.UNIT_PRICE_DISPLAY_DOLLARS = inputRecord.UNIT_PRICE_COMPUTATIONAL;
                inputRecord.WK_UNIT_PRICE = inputRecord.UNIT_PRICE_DISPLAY_DOLLARS + "/" + inputRecord.DISPLAY_UNITS;
            }
            else
            {
                inputRecord.UNIT_PRICE_COMPUTATIONAL = (inputRecord.UNIT_PRICE_COMPUTATIONAL.ConvertStringToDouble() * 100).ToString();
                inputRecord.UNIT_PRICE_COMPUTATIONAL = (inputRecord.UNIT_PRICE_COMPUTATIONAL.ConvertStringToDouble() + .099).ToString();

                inputRecord.UNIT_PRICE_DISPLAY_CENTS = inputRecord.UNIT_PRICE_COMPUTATIONAL;
                inputRecord.WK_UNIT_PRICE = inputRecord.UNIT_PRICE_DISPLAY_CENTS + "_`/" + inputRecord.DISPLAY_UNITS;
            }


            SlideUpLeft(inputRecord, records); // Function Call
            inputRecord.OS_UNIT_PRICE = inputRecord.WK_UNIT_PRICE;

        }

        /// <summary>
        ///Initializes data for Unit Price Processing
        /// </summary>
        /// <param name="inputRecord"></param>
        public void UnitPric_Main(InputFile inputRecord, InputRecords records)
        {

            if (!string.IsNullOrEmpty(inputRecord.OS_UNIT_PRICE.EmptyNull().Trim()))
            {
                inputRecord.PACKAGE_SIZE_DATA = string.Empty;
                inputRecord.PACKAGE_SIZE_DATA = inputRecord.OS_UNIT_PRICE.Split(new string[] { "   ", "  ", " " }, StringSplitOptions.None).ElementAtOrDefault(0);
                inputRecord.PS_SIZE = inputRecord.OS_UNIT_PRICE.Split(new string[] { "   ", "  ", " " }, StringSplitOptions.None).ElementAtOrDefault(1);
                inputRecord.PS_UNITS = inputRecord.OS_UNIT_PRICE.Split(new string[] { "   ", "  ", " " }, StringSplitOptions.None).ElementAtOrDefault(2);
                inputRecord.PS_UNITS2 = inputRecord.OS_UNIT_PRICE.Split(new string[] { "   ", "  ", " " }, StringSplitOptions.None).ElementAtOrDefault(3);

                inputRecord.PS_TYPE_CHAR1 = inputRecord.PACKAGE_SIZE_DATA.SafeSubstring(0, 1);
                inputRecord.PS_TYPE_CHAR2 = inputRecord.PACKAGE_SIZE_DATA.SafeSubstring(1, 1);

                if (inputRecord.OS_UNIT_PRICE.Split(new string[] { "   ", "  ", " " }, StringSplitOptions.None).Length == 1)
                {
                    inputRecord.PS_SIZE = inputRecord.PACKAGE_SIZE_DATA.SafeSubstring(22, 10);
                    inputRecord.PS_UNITS = inputRecord.PACKAGE_SIZE_DATA.SafeSubstring(2, 10);
                    inputRecord.PS_UNITS2 = inputRecord.PACKAGE_SIZE_DATA.SafeSubstring(12, 10);

                }

                if (!string.IsNullOrEmpty(inputRecord.PS_UNITS2))
                {
                    inputRecord.WORK_STRING = string.Empty;
                    inputRecord.WORK_STRING = inputRecord.PS_UNITS.Split(' ').ElementAtOrDefault(0) + " " + inputRecord.PS_UNITS2.Split(' ').ElementAtOrDefault(0);
                    inputRecord.PS_UNITS = inputRecord.WORK_STRING;
                    inputRecord.PS_UNITS2 = string.Empty;
                }

                inputRecord.COMPUTE_UNIT_PRICE_FLAG = Constants.N;
                inputRecord.S_I_INIT_FIELD = 0;

                if (inputRecord.PS_TYPE_CHAR1 == ".")
                {
                    inputRecord.PS_TYPE_CHAR2 = inputRecord.PS_TYPE_CHAR2.ToUpper();

                    if (Constants.ChkList_PS_TYPE_CHAR2.Contains(inputRecord.PS_TYPE_CHAR2))
                    {
                        UnitPric_ComputeUnitPrice(inputRecord, records);
                        inputRecord.S_I_REDISPLAY = "R";
                        inputRecord.S_I_DISP_FLD = 25;
                    }
                    else
                    {
                        inputRecord.S_I_INIT_FIELD = inputRecord.S_I_INIT_FIELD + 1;
                    }
                }
                else
                {
                    inputRecord.S_I_INIT_FIELD = inputRecord.S_I_INIT_FIELD + 1;
                }

            }
        }

        /// <summary>
        ///Left Align Data
        /// </summary>
        /// <param name="inputRecord"></param>
        public void SlideUpLeft(InputFile inputRecord, InputRecords records)
        {

            inputRecord.WORK_STRING = inputRecord.WORK_STRING.EmptyNull().TrimStart(' ');
            inputRecord.WK_UNIT_PRICE = inputRecord.WORK_STRING;
        }

        /// <summary>
        ///Adjust's Unit Price
        /// </summary>
        /// <param name="inputRecord"></param>
        public void AdjustUnitPrice(InputFile inputRecord, InputRecords records)
        {

            inputRecord.DISPLAY_UNITS = inputRecord.UPE_UNIT;
            if (inputRecord.UPE_DENOMINATOR.ConvertStringToDouble() > 0)
            {
                inputRecord.UNIT_PRICE_COMPUTATIONAL = ((inputRecord.UNIT_PRICE_COMPUTATIONAL.ConvertStringToDouble() * inputRecord.UPE_NUMERATOR.ConvertStringToDouble()) / inputRecord.UPE_DENOMINATOR.ConvertStringToDouble()).ToString();
            }
            else
            {
                inputRecord.UNIT_PRICE_COMPUTATIONAL = "0.0";
            }

        }


        /// <summary>
        ///Rejects's Unit Price units
        /// </summary>
        /// <param name="inputRecord"></param>
        public void RejectUnitPriceUnits(InputFile inputRecord, InputRecords records)
        {

            switch (inputRecord.PS_TYPE_CHAR2)
            {
                case "D":
                    inputRecord.PS_TYPE_CHAR2 = "L";
                    inputRecord.RE_SEARCH_FLAG = Constants.Y;
                    break;

                case "L":
                case "P":
                    inputRecord.PS_TYPE_CHAR2 = "C";
                    inputRecord.RE_SEARCH_FLAG = Constants.Y;
                    break;

                case "C":
                    inputRecord.PS_TYPE_CHAR2 = "F";
                    inputRecord.RE_SEARCH_FLAG = Constants.Y;
                    break;

                case "F":
                    inputRecord.PS_TYPE_CHAR2 = "E";
                    inputRecord.RE_SEARCH_FLAG = Constants.Y;
                    break;

                case "E":
                    inputRecord.PS_TYPE_CHAR2 = "A";
                    inputRecord.RE_SEARCH_FLAG = Constants.Y;
                    break;

                default:
                    inputRecord.RE_SEARCH_FLAG = Constants.N;
                    inputRecord.MSG_MESSAGE = string.Empty;
                    inputRecord.MSG_MESSAGE = "Unknown unit of measure in Package-Size: " + inputRecord.PS_KEY.EmptyNull().Split(new string[] { "  " }, StringSplitOptions.None).ElementAtOrDefault(0) + " - check Unit Price!";
                    inputRecord.MSG_CR = Constants.Y;
                    Common.CALL_MSGDRVR(inputRecord, packagepath);
                    break;
            }

        }

        /// <summary>
        ///Write Exceptions
        /// </summary>
        /// <param name="records"></param>
        /// <param name="lbl"></param>
        /// <param name="exceptionDescription"></param>
        /// <param name="fieldName"></param>
        private void WriteExceptions(InputFile lbl, string exceptionDescription, string fieldName, InputRecords records)
        {
            lbl.MSG_MESSAGE = exceptionDescription;
            ExceptionReport exceptions = new ExceptionReport();

            //Keep sign id as -1 if it is a child record
            exceptions.SignId = (InputFileList != null && InputFileList.Count > 0) ? -1 : lbl.SignDataId;
            exceptions.OrderId = String.IsNullOrEmpty(lbl.ORDER_ID) ? 0 : Helper.ConvertStringToInteger(lbl.ORDER_ID);
            exceptions.ExceptionDescription = exceptionDescription;
            exceptions.ExceptionStatus = "Information";
            exceptions.FieldName = fieldName;
            exceptions.RowNum = ProcessInput.recordNumber;
            records.Exceptions.Add(exceptions);
        }

    }
}

