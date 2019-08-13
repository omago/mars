using GraphvizSample;
using Mateus.Model.BussinesLogic.Views.ContractModel;
using Mateus.Model.BussinesLogic.Views.LegalEntityAuditModel;
using Mateus.Model.BussinesLogic.Views.LegalEntityBankAuditModel;
using Mateus.Model.BussinesLogic.Views.LegalEntityBankModel;
using Mateus.Model.BussinesLogic.Views.LegalEntityBranchAuditModel;
using Mateus.Model.BussinesLogic.Views.LegalEntityBranchModel;
using Mateus.Model.BussinesLogic.Views.LegalEntityLegalRepresentativeAuditModel;
using Mateus.Model.BussinesLogic.Views.LegalEntityLegalRepresentativeModel;
using Mateus.Model.BussinesLogic.Views.LegalEntityModel;
using Mateus.Model.BussinesLogic.Views.LegalEntityOwnerAuditModel;
using Mateus.Model.BussinesLogic.Views.LegalEntityOwnerModel;
using Mateus.Model.BussinesLogic.Views.WorkDoneModel;
using Mateus.Model.EFModel;
using Mateus.Model.EFModel.Repository.Concrete;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Support;
using OfficeOpenXml;
using PITFramework.Support;
using PITFramework.Support.Grid;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace Mateus.Controllers
{
    public class ReportController : Controller
    {
        #region Initalizers

        private List<int> legalEntitiesPKsToExclude;
        private List<int> workDonesPKPKsToExclude;
        private readonly Mateus_wcEntities db = null;
        public ReportController()
        {
            db = new Mateus_wcEntities();
        }

        #endregion

        [PITAuthorize(Roles = "add, edit, view, delete")]
        public ActionResult LegalEntities(string Name, string OIB, string MB, string MBS, int? BankPK, int? TaxPK, int? FormPK, int? ActivityPK, int? SubstationPK, int? CommercialCourtPK, int? NumberOfEmployeesFrom, int? NumberOfEmployeesTo, int? FundamentalCapitalFrom, int? FundamentalCapitalTo, string TouristOffice, string MonumentAnnuity)
        {
            string sortOrder = !string.IsNullOrWhiteSpace(Request.QueryString["sortOrder"]) ? Request.QueryString["sortOrder"] : "DESC";
            string sortColumn = !string.IsNullOrWhiteSpace(Request.QueryString["sortColumn"]) ? Request.QueryString["sortColumn"] : "LegalEntityPK";
            string ordering = sortColumn + " " + sortOrder;
            ordering = ordering.Trim();

            IBanksRepository banksRepository = new BanksRepository(db);
            ITaxesRepository taxesRepository = new TaxesRepository(db);
            IFormsRepository formsRepository = new FormsRepository(db);
            IActivitiesRepository activitiesRepository = new ActivitiesRepository(db);
            ISubstationsRepository substationsRepository = new SubstationsRepository(db);
            ICommercialCourtsRepository commercialCourtsRepository = new CommercialCourtsRepository(db);

            // Binding DDL for report search
            ViewBag.Banks = new SelectList(banksRepository.GetValid().OrderBy("Name ASC").ToList(), "BankPK", "Name", BankPK);
            ViewBag.Taxes = new SelectList(taxesRepository.GetValid().OrderBy("Name ASC").ToList(), "TaxPK", "Name", TaxPK);
            ViewBag.Forms = new SelectList(formsRepository.GetValid().OrderBy("Name ASC").ToList(), "FormPK", "Name", FormPK);
            ViewBag.Activities = new SelectList(activitiesRepository.GetValid().OrderBy("Name ASC").ToList(), "ActivityPK", "Name", ActivityPK);
            ViewBag.Substations = new SelectList(substationsRepository.GetValid().OrderBy("Name ASC").ToList(), "SubstationPK", "Name", SubstationPK);
            ViewBag.CommercialCourts = new SelectList(commercialCourtsRepository.GetValid().OrderBy("Name ASC").ToList(), "CommercialCourtPK", "Name", CommercialCourtPK);

            if (BankPK != null) { ViewBag.Bank = banksRepository.GetBankByPK((int)BankPK).Name; }
            if (TaxPK != null) { ViewBag.Tax = taxesRepository.GetTaxByPK((int)TaxPK).Name; }

            if (SubstationPK != null) { ViewBag.Substation = substationsRepository.GetSubstationByPK((int)SubstationPK).Name; }
            if (CommercialCourtPK != null) { ViewBag.CommercialCourt = commercialCourtsRepository.GetCommercialCourtByPK((int)CommercialCourtPK).Name; }

            if (FormPK != null) { ViewBag.Form = formsRepository.GetFormByPK((int)FormPK).Name; }
            if (ActivityPK != null) { ViewBag.Activity = activitiesRepository.GetActivityByPK((int)ActivityPK).Name; }

            bool? TouristOfficeFlag = null;
            bool? MonumentAnnuityFlag = null;

            if (TouristOffice == "on") { TouristOfficeFlag = true; }
            if (MonumentAnnuity == "on") { MonumentAnnuityFlag = true; }

            // Applying filters
            IQueryable<LegalEntityView> legalEntities = LegalEntityView.GetLegalEntitiesReport(db, Name, OIB, MB, MBS, BankPK, TaxPK, FormPK, ActivityPK, SubstationPK, CommercialCourtPK, NumberOfEmployeesFrom, NumberOfEmployeesTo, FundamentalCapitalFrom, FundamentalCapitalTo, TouristOfficeFlag, MonumentAnnuityFlag)
                                                                   .OrderBy(ordering);

            // Excluding temporary deleted items from view
            legalEntitiesPKsToExclude = new List<int>();

            // Empty session on first request
            if (Request.QueryString.Count == 0) { Session["legalEntitiesPKsToExclude"] = null; }

            if (Session["legalEntitiesPKsToExclude"] != null)
            {
                legalEntitiesPKsToExclude = (List<int>)Session["legalEntitiesPKsToExclude"];
                legalEntities = legalEntities.Where(c => !legalEntitiesPKsToExclude.Contains(c.LegalEntityPK));
            }

            return View("LegalEntities", legalEntities.ToList());
        }

        //[PITAuthorize(Roles = "add, edit, view, delete")]
        public ActionResult LegalEntity(int? legalEntityFK, string ShowBasicInfo, string ShowLegalEntityHistory, string ShowLegalEntityLegalRepresentatives, string ShowLegalEntityLegalRepresentativesHistory, string ShowLegalEntityBanks, string ShowLegalEntityBanksHistory, string ShowContracts, string ShowBranches, string ShowBranchesHistory, string ShowLegalEntityOwners, string ShowLegalEntityOwnersHistory)
        {
            LegalEntityView legalEntityView = new LegalEntityView();

            ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(db);

            if (legalEntityFK != null)
            {

                int legalEntityPK = (int)legalEntityFK;

                if (ShowBasicInfo == "on" || ShowBasicInfo == "true")
                {
                    legalEntityView = LegalEntityView.GetLegalEntityReport(db, legalEntityPK);
                }

                if (ShowLegalEntityLegalRepresentatives == "on" || ShowLegalEntityLegalRepresentatives == "true")
                {
                    IPhysicalEntitiesRepository physicalEntitiesRepository = new PhysicalEntitiesRepository(db);
                    ILegalEntityLegalRepresentativesRepository legalEntityLegalRepresentativesRepository = new LegalEntityLegalRepresentativesRepository(db);

                    IQueryable<LegalEntityLegalRepresentativeView> legalEntityLegalRepresentatives = LegalEntityLegalRepresentativeView.GetLegalEntityLegalRepresentativeView(legalEntityLegalRepresentativesRepository.GetValid(), legalEntitiesRepository.GetValidLegalEntities(), physicalEntitiesRepository.GetValid());

                    ViewBag.LegalEntityLegalRepresentatives = legalEntityLegalRepresentatives.Where(c => c.LegalEntityFK == legalEntityPK).ToList();
                }

                if (ShowLegalEntityBanks == "on" || ShowLegalEntityBanks == "true")
                {
                    ILegalEntityBanksRepository legalEntitiesBanksRepository = new LegalEntityBanksRepository(db);
                    IBanksRepository banksRepository = new BanksRepository(db);

                    IQueryable<LegalEntityBankView> legalEntitiesBanks = LegalEntityBankView.GetLegalEntityBankView(legalEntitiesBanksRepository.GetValid(), banksRepository.GetValid(), legalEntitiesRepository.GetValidLegalEntities());

                    ViewBag.LegalEntityBanks = legalEntitiesBanks.Where(c => c.LegalEntityFK == legalEntityPK).ToList();
                }

                if (ShowContracts == "on" || ShowContracts == "true")
                {
                    IContractsRepository contractsRepository = new ContractsRepository(db);
                    IAnnexContractsRepository annexContractsRepository = new AnnexContractsRepository(db);

                    IQueryable<ContractView> contracts = ContractView.GetContractsView(contractsRepository.GetValid(), annexContractsRepository.GetValid(), legalEntitiesRepository.GetValidLegalEntities());

                    ViewBag.Contracts = contracts.Where(c => c.LegalEntityFK == legalEntityPK).ToList();
                }

                if (ShowBranches == "on" || ShowBranches == "true")
                {
                    ILegalEntityBranchesRepository legalEntityBranchesRepository = new LegalEntityBranchesRepository(db);

                    IQueryable<LegalEntityBranchView> legalEntityBranches = LegalEntityBranchView.GetLegalEntityBranchView(legalEntityBranchesRepository.GetValid(), legalEntitiesRepository.GetValidLegalEntities());

                    ViewBag.Branches = legalEntityBranches.Where(c => c.LegalEntityFK == legalEntityPK).ToList();
                }

                // history
                if (ShowLegalEntityHistory == "on" || ShowLegalEntityHistory == "true")
                {
                    List<LegalEntityAuditView> legalEntityHistory = LegalEntityAuditView.GetLegalEntityAuditView(db, legalEntityPK);
                    ViewBag.legalEntityHistory = legalEntityHistory.ToList();
                }

                if (ShowLegalEntityBanksHistory == "on" || ShowLegalEntityBanksHistory == "true")
                {
                    List<List<LegalEntityBankAuditView>> legalEntityBanksHistory = LegalEntityBankAuditView.GetLegalEntityBanksAuditView(db, legalEntityPK);

                    List<DateTime> legalEntityBanksDatesHistory = new List<DateTime>();

                    foreach (List<LegalEntityBankAuditView> legalEntityBank in legalEntityBanksHistory)
                    {
                        foreach (LegalEntityBankAuditView legalEntityBankAuditView in legalEntityBank)
                        {
                            if (!legalEntityBanksDatesHistory.Contains(legalEntityBankAuditView.ChangeDate.Value))
                            {
                                legalEntityBanksDatesHistory.Add(legalEntityBankAuditView.ChangeDate.Value);
                            }
                        }
                    }

                    ViewBag.legalEntityBanksDatesHistory = legalEntityBanksDatesHistory.OrderBy(c => c.Date).ToList();
                    ViewBag.legalEntityBanksHistory = legalEntityBanksHistory;
                }

                if (ShowLegalEntityLegalRepresentativesHistory == "on" || ShowLegalEntityLegalRepresentativesHistory == "true")
                {
                    List<List<LegalEntityLegalRepresentativeAuditView>> legalEntityLegalRepresentativesHistory = LegalEntityLegalRepresentativeAuditView.GetLegalEntityLegalRepresentativesAuditView(db, legalEntityPK);
                    ViewBag.legalEntityLegalRepresentativesHistory = legalEntityLegalRepresentativesHistory;
                }

                if (ShowLegalEntityOwnersHistory == "on" || ShowLegalEntityOwnersHistory == "true")
                {
                    List<List<LegalEntityOwnerAuditView>> legalEntityOwnersHistory = LegalEntityOwnerAuditView.GetLegalEntityOwnersAuditView(db, legalEntityPK);
                    ViewBag.legalEntityOwnersHistory = legalEntityOwnersHistory;
                }

                if (ShowBranchesHistory == "on" || ShowBranchesHistory == "on")
                {
                    List<List<LegalEntityBranchAuditView>> legalEntityBranchesHistory = LegalEntityBranchAuditView.GetLegalEntityBranchesAuditView(db, legalEntityPK);
                    ViewBag.legalEntityBranchesHistory = legalEntityBranchesHistory;
                }

            }

            return View(legalEntityView);
        }

        [PITAuthorize(Roles = "add, edit, view, delete")]
        public ActionResult WorkDone(int? toDoListFK, int? legalEntityFK, int? workTypeFK, int? workSubtypeFK, int? serviceTypeFK, int? userFK, string dateFrom, string dateTo, int? timeSpentFrom, int? timeSpentTo, int? numberOfAttachmentsFrom, int? numberOfAttachmentsTo, string description)
        {
            IWorkDonesRepository workDonesRepository = new WorkDonesRepository(db);
            IWorkDoneAttachmentsRepository workDoneAttachmentsRepository = new WorkDoneAttachmentsRepository(db);
            WorkDoneView workDoneView = new WorkDoneView();
            IContractsRepository contractsRepository = new ContractsRepository(db);
            ILegalEntityBranchesRepository legalEntityBranchesRepository = new LegalEntityBranchesRepository(db);
            IToDoListsRepository toDoListsRepository = new ToDoListsRepository(db);
            ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(db);
            IWorkTypesRepository workTypesRepository = new WorkTypesRepository(db);
            IWorkSubtypesRepository workSubtypesRepository = new WorkSubtypesRepository(db);
            IServiceTypesRepository serviceTypesRepository = new ServiceTypesRepository(db);
            IUsersRepository usersRepository = new UsersRepository(db);

            string sortOrder = !string.IsNullOrWhiteSpace(Request.QueryString["sortOrder"]) ? Request.QueryString["sortOrder"] : "DESC";
            string sortColumn = !string.IsNullOrWhiteSpace(Request.QueryString["sortColumn"]) ? Request.QueryString["sortColumn"] : "WorkDonePK";
            string ordering = sortColumn + " " + sortOrder;
            ordering = ordering.Trim();

            //grid filters ddl
            ViewBag.ToDoLists = new SelectList(toDoListsRepository.GetValid().OrderBy("Name ASC").ToList(), "ToDoListPK", "Name", toDoListFK);
            ViewBag.LegalEntities = new SelectList(legalEntitiesRepository.GetValidLegalEntities().OrderBy("Name ASC").ToList(), "LegalEntityPK", "Name", legalEntityFK);
            ViewBag.WorkTypes = new SelectList(workTypesRepository.GetValid().OrderBy("Name ASC").ToList(), "WorkTypePK", "Name", workTypeFK);
            ViewBag.WorkSubtypes = new SelectList(workSubtypesRepository.GetValid().OrderBy("Name ASC").ToList(), "WorkSubtypePK", "Name", workSubtypeFK);
            ViewBag.ServiceTypes = new SelectList(serviceTypesRepository.GetValid().OrderBy("Name ASC").ToList(), "ServiceTypePK", "Name", serviceTypeFK);
            ViewBag.Users = new SelectList(usersRepository.GetValid().OrderBy("Username ASC").ToList(), "UserPK", "Username", userFK);

            if (legalEntityFK != null) { ViewBag.LegalEntity = legalEntitiesRepository.GetLegalEntityByPK((int)legalEntityFK).Name; }
            if (toDoListFK != null) { ViewBag.ToDoList = toDoListsRepository.GetToDoListByPK((int)toDoListFK).Name; }
            if (workTypeFK != null) { ViewBag.WorkType = workTypesRepository.GetWorkTypeByPK((int)workTypeFK).Name; }
            if (workSubtypeFK != null) { ViewBag.WorkSubtype = workSubtypesRepository.GetWorkSubtypeByPK((int)workSubtypeFK).Name; }
            if (userFK != null) { ViewBag.User = usersRepository.GetUserByUserID((int)userFK).Username; }

            DateTime? dateFromTime = null;
            DateTime? dateToTime = null;

            if (dateFrom != null && dateFrom != "") { dateFromTime = DateTime.ParseExact(dateFrom, "dd.MM.yyyy.", null); }
            if (dateTo != null && dateTo != "") { dateToTime = DateTime.ParseExact(dateTo, "dd.MM.yyyy.", null); }

            // Applying filters
            IQueryable<WorkDone> workDonesFiltered = WorkDoneView.GetWorkDonesReport(db, toDoListFK, legalEntityFK, workTypeFK, workSubtypeFK, serviceTypeFK, userFK, dateFromTime, dateToTime, timeSpentFrom, timeSpentTo, numberOfAttachmentsFrom, numberOfAttachmentsTo, description);

            IQueryable<WorkDoneView> workDones = WorkDoneView.GetWorkDoneView(workDonesFiltered,
                                                                               toDoListsRepository.GetValid(),
                                                                               workDoneAttachmentsRepository.GetValid(),
                                                                               legalEntitiesRepository.GetValidLegalEntities(),
                                                                               workTypesRepository.GetValid(),
                                                                               workSubtypesRepository.GetValid(),
                                                                               serviceTypesRepository.GetValid(),
                                                                               usersRepository.GetValid())
                                                             .OrderBy(ordering);

            // Excluding temporary deleted items from view
            workDonesPKPKsToExclude = new List<int>();

            // Empty session on first request
            if (Request.QueryString.Count == 0) { Session["workDonesPKPKsToExclude"] = null; }

            if (Session["workDonesPKPKsToExclude"] != null)
            {
                workDonesPKPKsToExclude = (List<int>)Session["workDonesPKPKsToExclude"];
                workDones = workDones.Where(c => !workDonesPKPKsToExclude.Contains(c.WorkDonePK));
            }

            return View("WorkDone", workDones.ToList());
        }



        public FileContentResult ShowLegalEntityOwnersGraph(int? legalEntityFK, string color, int? dpi)
        {
            ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(db);

            ILegalEntityOwnersRepository legalEntityOwnersRepository = new LegalEntityOwnersRepository(db);
            IPhysicalEntitiesRepository physicalEntitiesRepository = new PhysicalEntitiesRepository(db);

            List<LegalEntityOwner> filteredCoList = new List<LegalEntityOwner>();
            IQueryable<LegalEntityOwner> legalEntityOwnersTable = legalEntityOwnersRepository.GetValid();

            if (legalEntityFK != null)
            {
                List<LegalEntityOwner> coList = new List<LegalEntityOwner>();
                coList = legalEntityOwnersRepository.GetFirstLegalEntityOwnersForLegalEntity((int)legalEntityFK).ToList();

                foreach (var co in coList)
                {
                    LegalEntityOwnerView.Fill(filteredCoList, legalEntityOwnersTable.ToList(), co);
                }

                legalEntityOwnersTable = filteredCoList.AsQueryable();
            }

            IQueryable<LegalEntityOwnerView> legalEntityOwners = LegalEntityOwnerView.GetLegalEntityOwnerView(legalEntityOwnersTable,
                                                                                               physicalEntitiesRepository.GetValid(),
                                                                                               legalEntitiesRepository.GetValid());

            List<string> graphElements = new List<string>();

            foreach (LegalEntityOwnerView item in legalEntityOwners)
            {
                graphElements.Add(item.OwnerFK.ToString() + "[label=\"" + item.OwnerName + "\", shape=ci, fontsize=12, fontname=arial, labelloc=t, style=filled, fillcolor=\"white\", width=1.5]");

                if (legalEntityFK == item.LegalEntityFK)
                {
                    graphElements.Add(item.LegalEntityFK.ToString() + "[label=\"" + item.LegalEntityName + "\", shape=box, fontsize=12, fontname=arial, labelloc=t, style=filled, fontcolor=\"white\", color=\"black\", width=1.5]");
                }
                else
                {
                    graphElements.Add(item.LegalEntityFK.ToString() + "[label=\"" + item.LegalEntityName + "\", shape=box, fontsize=12, fontname=arial, labelloc=t, style=filled, fillcolor=\"white\", width=1.5]");
                }

                graphElements.Add(item.OwnerFK.ToString() + "->" + item.LegalEntityFK.ToString());
            }

            string graph = @"digraph Graphviz {";
            graph += "[bgcolor=\"" + color + "\", dpi=\"" + dpi + "\"],";
            graph += string.Join(", ", graphElements.ToArray());
            graph += "}";

            graph = graph.ConvertNonASCIICharacters();

            byte[] imageByte = imageByte = Graphviz.RenderImage(graph, "dot", "png"); ;
            string contentType = "image/png";

            return File(imageByte, contentType);
        }

        public ActionResult WorkDoneExportToExcel(
            int? toDoListFK, int? legalEntityFK, int? workTypeFK, int? workSubtypeFK, int? serviceTypeFK, int? userFK, string dateFrom, string dateTo, int? timeSpentFrom, int? timeSpentTo, int? numberOfAttachmentsFrom, int? numberOfAttachmentsTo, string description,
            bool ShowBasicInfo, bool ShowOrdinal, bool ShowID, bool ShowToDoList, bool ShowLegalEntity, bool ShowWorkType, bool ShowWorkSubtype, bool ShowServiceType, bool ShowDate, bool ShowUsername, bool ShowDescription, bool ShowTimeSpent, bool ShowComment, bool ShowAttachments)
        {
            IWorkDonesRepository workDonesRepository = new WorkDonesRepository(db);
            IWorkDoneAttachmentsRepository workDoneAttachmentsRepository = new WorkDoneAttachmentsRepository(db);
            WorkDoneView workDoneView = new WorkDoneView();
            IContractsRepository contractsRepository = new ContractsRepository(db);
            ILegalEntityBranchesRepository legalEntityBranchesRepository = new LegalEntityBranchesRepository(db);
            IToDoListsRepository toDoListsRepository = new ToDoListsRepository(db);
            ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(db);
            IWorkTypesRepository workTypesRepository = new WorkTypesRepository(db);
            IWorkSubtypesRepository workSubtypesRepository = new WorkSubtypesRepository(db);
            IServiceTypesRepository serviceTypesRepository = new ServiceTypesRepository(db);
            IUsersRepository usersRepository = new UsersRepository(db);

            string sortOrder = !string.IsNullOrWhiteSpace(Request.QueryString["sortOrder"]) ? Request.QueryString["sortOrder"] : "DESC";
            string sortColumn = !string.IsNullOrWhiteSpace(Request.QueryString["sortColumn"]) ? Request.QueryString["sortColumn"] : "WorkDonePK";
            string ordering = sortColumn + " " + sortOrder;
            ordering = ordering.Trim();

            if (legalEntityFK != null) { ViewBag.LegalEntity = legalEntitiesRepository.GetLegalEntityByPK((int)legalEntityFK).Name; }
            if (toDoListFK != null) { ViewBag.ToDoList = toDoListsRepository.GetToDoListByPK((int)toDoListFK).Name; }
            if (workTypeFK != null) { ViewBag.WorkType = workTypesRepository.GetWorkTypeByPK((int)workTypeFK).Name; }
            if (workSubtypeFK != null) { ViewBag.WorkSubtype = workSubtypesRepository.GetWorkSubtypeByPK((int)workSubtypeFK).Name; }
            if (userFK != null) { ViewBag.User = usersRepository.GetUserByUserID((int)userFK).Username; }

            DateTime? dateFromTime = null;
            DateTime? dateToTime = null;

            if (dateFrom != null && dateFrom != "") { dateFromTime = DateTime.ParseExact(dateFrom, "dd.MM.yyyy.", null); }
            if (dateTo != null && dateTo != "") { dateToTime = DateTime.ParseExact(dateTo, "dd.MM.yyyy.", null); }

            // Applying filters
            IQueryable<WorkDone> workDonesFiltered = WorkDoneView.GetWorkDonesReport(db, toDoListFK, legalEntityFK, workTypeFK, workSubtypeFK, serviceTypeFK, userFK, dateFromTime, dateToTime, timeSpentFrom, timeSpentTo, numberOfAttachmentsFrom, numberOfAttachmentsTo, description);

            IQueryable<WorkDoneView> workDones = WorkDoneView.GetWorkDoneView(workDonesFiltered,
                                                                               toDoListsRepository.GetValid(),
                                                                               workDoneAttachmentsRepository.GetValid(),
                                                                               legalEntitiesRepository.GetValidLegalEntities(),
                                                                               workTypesRepository.GetValid(),
                                                                               workSubtypesRepository.GetValid(),
                                                                               serviceTypesRepository.GetValid(),
                                                                               usersRepository.GetValid())
                                                             .OrderBy(ordering);

            var workDonesList = workDones.ToList();

            var columns = new Dictionary<string, int>();

            var ms = new MemoryStream();
            using (var package = new ExcelPackage(ms))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Izvršeni posao");

                var columnPosition = 1;

                Action<bool, string> setColumnHeader = (columnVisible, columnName) => 
                {
                    if (columnVisible)
                    {
                        worksheet.Cells[1, columnPosition].Value = columnName;
                        worksheet.Cells[1, columnPosition].Style.Font.Bold = true;
                        columns.Add(columnName, columnPosition++);
                    }
                };

                setColumnHeader(ShowOrdinal, "#");
                setColumnHeader(ShowID, "ID");
                setColumnHeader(ShowToDoList, "Obaveza");
                setColumnHeader(ShowLegalEntity, "Tvrtka");
                setColumnHeader(ShowWorkType, "Vrsta rada");
                setColumnHeader(ShowWorkSubtype, "Vrsta posla");
                setColumnHeader(ShowServiceType, "Vrsta usluge");
                setColumnHeader(ShowDate, "Datum izvršenja");
                setColumnHeader(ShowUsername, "Korisnik");
                setColumnHeader(ShowDescription, "Opis");
                setColumnHeader(ShowTimeSpent, "Utrošeno vrijeme");
                setColumnHeader(ShowComment, "Važna napomena");
                setColumnHeader(ShowAttachments, "Prilozi");

                Action<int, bool, string, object> setRowValue = (ri, columnVisible, columnName, value) =>
                {
                    if (columnVisible)
                    {
                        worksheet.Cells[ri, columns[columnName]].Value = value;
                    }
                };

                var rowIndex = 2;
                foreach (var item in workDonesList)
                {
                    setRowValue(rowIndex, ShowOrdinal, "#", rowIndex - 1);
                    setRowValue(rowIndex, ShowID, "ID", item.WorkDonePK);
                    setRowValue(rowIndex, ShowToDoList, "Obaveza", item.ToDoListName);
                    setRowValue(rowIndex, ShowLegalEntity, "Tvrtka", item.LegalEntityName);
                    setRowValue(rowIndex, ShowWorkType, "Vrsta rada", item.WorkTypeName);
                    setRowValue(rowIndex, ShowWorkSubtype, "Vrsta posla", item.WorkSubtypeName);
                    setRowValue(rowIndex, ShowServiceType, "Vrsta usluge", item.ServiceTypeName);
                    setRowValue(rowIndex, ShowDate, "Datum izvršenja", item.Date.Value.ToString("dd.MM.yyyy."));
                    setRowValue(rowIndex, ShowUsername, "Korisnik",  item.UserUsername);
                    setRowValue(rowIndex, ShowDescription, "Opis", item.Description);
                    setRowValue(rowIndex, ShowTimeSpent, "Utrošeno vrijeme", LinkHelper.calculateTimeSpent(item.TimeSpent));
                    setRowValue(rowIndex, ShowComment, "Važna napomena", item.Comment);
                    setRowValue(rowIndex, ShowAttachments, "Prilozi", item.WorkDoneAttachmentsCount);

                    rowIndex++;
                }

                worksheet.Calculate();
                worksheet.Cells.AutoFitColumns(0);

                package.Save();
            }

            ms.Seek(0, SeekOrigin.Begin);

            return new FileStreamResult(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = $"Izvršeni posao {DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss")}.xlsx"
            };
        }

        [PITAuthorize(Roles = "delete")]
        public ActionResult DeleteTemporaryLegalEntities(int? legalEntityPK)
        {
            if (legalEntityPK != null)
            {
                legalEntitiesPKsToExclude = new List<int>();

                if (Session["legalEntitiesPKsToExclude"] != null) legalEntitiesPKsToExclude = (List<int>)Session["legalEntitiesPKsToExclude"];

                legalEntitiesPKsToExclude.Add((int)legalEntityPK);
                Session["legalEntitiesPKsToExclude"] = legalEntitiesPKsToExclude;
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        [PITAuthorize(Roles = "delete")]
        public ActionResult DeleteTemporaryWorkDone(int? workDonePK)
        {
            if (workDonePK != null)
            {
                workDonesPKPKsToExclude = new List<int>();

                if (Session["workDonesPKPKsToExclude"] != null) workDonesPKPKsToExclude = (List<int>)Session["workDonesPKPKsToExclude"];

                workDonesPKPKsToExclude.Add((int)workDonePK);
                Session["workDonesPKPKsToExclude"] = workDonesPKPKsToExclude;
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        [PITAuthorize(Roles = "add, edit, view, delete")]
        public ActionResult ClearReport()
        {
            Session["legalEntitiesPKsToExclude"] = null;
            Session["workDonePKsToExclude"] = null;

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }
    }
}
