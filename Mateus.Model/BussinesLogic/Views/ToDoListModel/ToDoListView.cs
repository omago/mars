using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Mateus.Model.EFModel;
using System.Web.Mvc;

namespace Mateus.Model.BussinesLogic.Views.ToDoListModel
{
    public class ToDoListView
    {
        public int ToDoListPK { get; set; }

        [Required(ErrorMessage = "Naziv je obavezan."), StringLength(256, ErrorMessage = "Naziv nemože biti duži od 256 znakova.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Datum liste je obavezan.")]
        public DateTime? Date { get; set; }

        [Required(ErrorMessage = "Rok obaveze je obavezan.")]
        public DateTime? Deadline { get; set; }

        public int? UserFK { get; set; }
        public DateTime? CreationDate { get; set; }

        public int DeadlineHours { get; set; }
        public int DeadlineMinutes { get; set; }

        public bool? Finished { get; set; }
        public bool? Deleted { get; set; }

        public int? LegalEntityPK { get; set; }

        public IEnumerable<SelectListItem> Companies { get; set; }
        public IEnumerable<SelectListItem> Hours { get; set; }
        public IEnumerable<SelectListItem> Minutes { get; set; }

        public int? WorkDoneCreatedWithToDoCount { get; set; }
        public int? WorkDoneCount { get; set; }
        public string UserUsername { get; set; }

        public void ConvertFrom(ToDoList toDoList, ToDoListView toDoListView) 
        {
            toDoListView.ToDoListPK = toDoList.ToDoListPK;
            toDoListView.Name = toDoList.Name;
            toDoListView.Deadline = toDoList.Deadline;
            toDoListView.Date = toDoList.Date;
            toDoListView.UserFK = toDoList.UserFK;
            toDoListView.CreationDate = toDoList.CreationDate;
            toDoListView.Finished = toDoList.Finished;
            toDoListView.Deleted = toDoList.Deleted;
        }

        public void ConvertTo(ToDoListView toDoListView, ToDoList toDoList) 
        {
            toDoList.ToDoListPK = toDoListView.ToDoListPK;
            toDoList.Name = toDoListView.Name;
            toDoList.Deadline = toDoListView.Deadline;
            toDoList.Date = toDoListView.Date;
            toDoList.Finished = toDoListView.Finished;
            toDoList.Deleted = toDoListView.Deleted;
        }

        public static IQueryable<ToDoListView> GetToDoListView(IQueryable<ToDoList> toDoListTable, IQueryable<WorkDone> workDoneTable, IQueryable<User> userTable) 
        {
            IQueryable<ToDoListView> toDoListViewList = (from t1 in toDoListTable
                                                         join t2 in userTable on t1.UserFK equals t2.UserPK
                                       select new ToDoListView
                                       {
                                            ToDoListPK                      = t1.ToDoListPK,
                                            Name                            = t1.Name,
                                            Deadline                        = t1.Deadline,
                                            Date                            = t1.Date,
                                            CreationDate                    = t1.CreationDate,
                                            UserUsername                    = t2.Username,
                                            Finished                        = t1.Finished,
                                            WorkDoneCreatedWithToDoCount    = workDoneTable.Where(b => b.ToDoListFK == t1.ToDoListPK && b.CreatedWithToDo == true).Count(),
                                            WorkDoneCount                   = workDoneTable.Where(b => b.ToDoListFK == t1.ToDoListPK && (b.CreatedWithToDo == false || b.CreatedWithToDo == null)).Count(),
                                       }).AsQueryable<ToDoListView>();

            return toDoListViewList;
        }
    }
}
