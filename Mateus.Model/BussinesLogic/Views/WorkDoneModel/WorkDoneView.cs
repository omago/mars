using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Mateus.Model.EFModel;
using System.Web.Mvc;
using System.Data.Objects;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using Mateus.Model.EFModel.Repository;
using Mateus.Model.BussinesLogic.Support.Validation;

namespace Mateus.Model.BussinesLogic.Views.WorkDoneModel
{
    public class WorkDoneView
    {
        public int WorkDonePK { get; set; }

        [Required(ErrorMessage = "Obaveza je obavezna.")]
        public int? ToDoListFK { get; set; }

        [Required(ErrorMessage = "Tvrtka je obavezna.")]
        public int? LegalEntityFK { get; set; }

        [Required(ErrorMessage = "Datum  je obavezan."), LessThenSystemDate(ErrorMessage = "Datum ne može biti manji od trenutnog datuma.")]
        public DateTime? Date { get; set; }

        [Required(ErrorMessage = "Vrsta rada je obavezna.")]
        public int? WorkTypeFK { get; set; }

        [Required(ErrorMessage = "Vrsta posla je obavezna.")]
        public int? WorkSubtypeFK { get; set; }

        [Required(ErrorMessage = "Vrsta usluge je obavezna.")]
        public int? ServiceTypeFK { get; set; }

        [Required(ErrorMessage = "Opis posla je obavezan"), StringLength(1024, ErrorMessage = "Opis posla ne smije biti duži od 1024 znakova.")]
        public string Description { get; set; }

        [StringLength(1024, ErrorMessage = "Napomena ne može biti duža od 1024 znakova.")]
        public string Comment { get; set; }

        public int? UserFK { get; set; }

        public DateTime? CreationDate { get; set; }

        public int? TimeSpent { get; set; }

        [Required(ErrorMessage = "Sati utrošenog vremena su ubavezni.")]
        public int? TimeSpentHours { get; set; }

        [Required(ErrorMessage = "Minute utrošenog vremena su obavezne.")]
        public int? TimeSpentMinutes { get; set; }
        
        public bool? CreatedWithToDo { get; set; }

        public bool? Deleted { get; set; }

        public IEnumerable<SelectListItem> ToDoLists { get; set; }
        public IEnumerable<SelectListItem> LegalEntities { get; set; }
        public IEnumerable<SelectListItem> WorkTypes { get; set; }
        public IEnumerable<SelectListItem> WorkSubtypes { get; set; }
        public IEnumerable<SelectListItem> ServiceTypes { get; set; }
        public IEnumerable<SelectListItem> Hours { get; set; }
        public IEnumerable<SelectListItem> Minutes { get; set; }

        public string ToDoListName { get; set; }
        public int? WorkDoneAttachmentsCount { get; set; }
        public string LegalEntityName { get; set; }
        public string WorkTypeName { get; set; }
        public string WorkSubtypeName { get; set; }
        public string ServiceTypeName { get; set; }
        public string UserUsername { get; set; }

        public void ConvertFrom(WorkDone workDone, WorkDoneView workDoneView) 
        {
            int timeSpentHours = workDone.TimeSpent == null ? 0 : (int)workDone.TimeSpent / 60;
            int timeSpentMinutes = (workDone.TimeSpent == null ? 0 : (int)workDone.TimeSpent) - (timeSpentHours*60);

            workDoneView.WorkDonePK         = workDone.WorkDonePK;
            workDoneView.ToDoListFK         = workDone.ToDoListFK;
            workDoneView.LegalEntityFK      = workDone.LegalEntityFK;
            workDoneView.Date               = workDone.Date;
            workDoneView.WorkTypeFK         = workDone.WorkTypeFK;
            workDoneView.WorkSubtypeFK      = workDone.WorkSubtypeFK;
            workDoneView.ServiceTypeFK      = workDone.ServiceTypeFK;
            workDoneView.Description        = workDone.Description;
            workDoneView.Comment            = workDone.Comment;
            workDoneView.UserFK             = workDone.UserFK;
            workDoneView.CreationDate       = workDone.CreationDate;
            workDoneView.TimeSpentHours     = timeSpentHours;
            workDoneView.TimeSpentMinutes   = timeSpentMinutes;
            workDoneView.CreatedWithToDo    = workDone.CreatedWithToDo;
            workDoneView.Deleted            = workDone.Deleted;
        }

        public void ConvertTo(WorkDoneView workDoneView, WorkDone workDone) 
        {
            int? timeSpentHours = workDoneView.TimeSpentHours == null ? 0 : workDoneView.TimeSpentHours;
            int? timeSpentMinutes = workDoneView.TimeSpentMinutes == null ? 0 : workDoneView.TimeSpentMinutes;

            workDone.WorkDonePK         = workDoneView.WorkDonePK;
            workDone.ToDoListFK         = workDoneView.ToDoListFK;
            workDone.LegalEntityFK      = workDoneView.LegalEntityFK;
            workDone.Date               = workDoneView.Date;
            workDone.WorkTypeFK         = workDoneView.WorkTypeFK;
            workDone.WorkSubtypeFK      = workDoneView.WorkSubtypeFK;
            workDone.ServiceTypeFK      = workDoneView.ServiceTypeFK;
            workDone.Description        = workDoneView.Description;
            workDone.Comment            = workDoneView.Comment;
            workDone.TimeSpent          = timeSpentHours * 60 + timeSpentMinutes;
            workDone.CreatedWithToDo    = workDoneView.CreatedWithToDo;
            workDone.Deleted            = workDoneView.Deleted;
        }

        public static IQueryable<WorkDoneView> GetWorkDoneView(IQueryable<WorkDone> workDoneTable, IQueryable<ToDoList> toDoListTable, IQueryable<WorkDoneAttachment> workDoneAttachmentTable, IQueryable<LegalEntity> legalEntityTable, IQueryable<WorkType> workTypeTable, IQueryable<WorkSubtype> workSubtypeTable, IQueryable<ServiceType> serviceTypeTable, IQueryable<User> userTable) 
        {
            IQueryable<WorkDoneView> workDoneViewList = (from t1 in workDoneTable
                                       join t2 in toDoListTable on t1.ToDoListFK equals t2.ToDoListPK
                                       join t3 in legalEntityTable on t1.LegalEntityFK equals t3.LegalEntityPK
                                       from t4 in workTypeTable.Where(tbl => tbl.WorkTypePK == t1.WorkTypeFK).DefaultIfEmpty()
                                       from t5 in workSubtypeTable.Where(tbl => tbl.WorkSubtypePK == t1.WorkSubtypeFK).DefaultIfEmpty()
                                       from t6 in userTable.Where(tbl => tbl.UserPK == t1.UserFK).DefaultIfEmpty()
                                       from t7 in serviceTypeTable.Where(tbl => tbl.ServiceTypePK == t1.ServiceTypeFK).DefaultIfEmpty()
                                       select new WorkDoneView
                                       {
                                            WorkDonePK                  = t1.WorkDonePK,
                                            Description                 = t1.Description,
                                            Comment                     = t1.Comment,
                                            LegalEntityFK               = t1.LegalEntityFK,
                                            Date                        = t1.Date,
                                            CreationDate                = t1.CreationDate,
                                            UserFK                      = t1.UserFK,
                                            WorkTypeFK                  = t1.WorkTypeFK,
                                            WorkSubtypeFK               = t1.WorkSubtypeFK,
                                            UserUsername                = t6.Username, 
                                            TimeSpent                   = t1.TimeSpent,
                                            CreatedWithToDo             = t1.CreatedWithToDo,
                                            LegalEntityName             = t3.Name,
                                            WorkTypeName                = t4.Name,
                                            WorkSubtypeName             = t5.Name,
                                            ServiceTypeName             = t7.Name,
                                            ToDoListName                = t2.Name,
                                            ToDoListFK                  = t1.ToDoListFK,
                                            WorkDoneAttachmentsCount    = workDoneAttachmentTable.Where(c => c.WorkDoneFK == t1.WorkDonePK && (c.Deleted == null || c.Deleted == false)).Count(),
                                       }).AsQueryable<WorkDoneView>();

            return workDoneViewList;
        }

        public static IQueryable<WorkDone> GetWorkDonesReport(ObjectContext db, int? toDoListFK, int? legalEntityFK, int? workTypeFK, int? workSubtypeFK, int? serviceTypeFK, int? userFK, DateTime? dateFrom, DateTime? dateTo, int? timeSpentFrom, int? timeSpentTo, int? numberOfAttachmentsFrom, int? numberOfAttachmentsTo, string description)
        {
            IWorkDonesRepository workDonesRepository = new WorkDonesRepository(db);
            IWorkDoneAttachmentsRepository workDoneAttachmentsRepository = new WorkDoneAttachmentsRepository(db);

            IQueryable<WorkDone> workDonesFiltered = workDonesRepository.GetValid();

            if (toDoListFK != null)
            {
                workDonesFiltered = workDonesFiltered.GetWorkDonesByToDoList(toDoListFK);
            }

            if (workDonesFiltered.Count() > 0 && legalEntityFK != null)
            {
                workDonesFiltered = workDonesFiltered.GetWorkDoneByLegalEntity(legalEntityFK);
            }

            if (workDonesFiltered.Count() > 0 && workTypeFK != null)
            {
                workDonesFiltered = workDonesFiltered.GetWorkDoneByWorkType(workTypeFK);
            }

            if (workDonesFiltered.Count() > 0 && workSubtypeFK != null)
            {
                workDonesFiltered = workDonesFiltered.GetWorkDonesByWorkSubtype(workSubtypeFK);
            }

            if (workDonesFiltered.Count() > 0 && serviceTypeFK != null)
            {
                workDonesFiltered = workDonesFiltered.GetWorkDonesByServiceType(serviceTypeFK);
            }

            if (workDonesFiltered.Count() > 0 && userFK != null)
            {
                workDonesFiltered = workDonesFiltered.GetWorkDonesByUser(userFK);
            }

            if (workDonesFiltered.Count() > 0 && dateFrom != null)
            {
                workDonesFiltered = workDonesFiltered.GetWorkDonesByDateFrom(dateFrom);
            }

            if (workDonesFiltered.Count() > 0 && dateTo != null)
            {
                workDonesFiltered = workDonesFiltered.GetWorkDonesByDateTo(dateTo);
            }

            if (workDonesFiltered.Count() > 0 && (timeSpentFrom != null || timeSpentTo != null))
            {
                workDonesFiltered = workDonesFiltered.GetWorkDonesByTimeSpentRange(timeSpentFrom, timeSpentTo);
            }

            if (workDonesFiltered.Count() > 0 && (numberOfAttachmentsFrom != null || numberOfAttachmentsTo != null))
            {
                IQueryable<WorkDoneView> numberOfAttachmentsList = workDoneAttachmentsRepository.GetWorkDoneAttachmentCountByWorkDone(null);

                workDonesFiltered = workDonesFiltered.GetWorkDonesByNumberOfAttachmentsRange(numberOfAttachmentsList, numberOfAttachmentsFrom, numberOfAttachmentsTo);
            }

            if (workDonesFiltered.Count() > 0 && !String.IsNullOrWhiteSpace(description))
            {
                workDonesFiltered = workDonesFiltered.GetWorkDonesByDescription(description);
            }

            return workDonesFiltered;
        }

    }
}