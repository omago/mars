using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Mateus.Model.EFModel;
using System.Web.Mvc;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using System.Data.Objects;

namespace Mateus.Model.BussinesLogic.Views.WorkSubtypeModel
{
    public class WorkSubtypeView
    {
        public int WorkSubtypePK { get; set; }

        [Required(ErrorMessage = "Naziv je obavezan."), StringLength(256, ErrorMessage = "Naziv ne smije biti duži od 256 znakova.")]
        public string Name { get; set; }
  
        [Required(ErrorMessage = "Tip posla je obavezan.")]
        public int? WorkTypeFK { get; set; }
        
        public bool? Deleted { get; set; }

        public IEnumerable<SelectListItem> WorkTypes { get; set; }

        public string WorkTypeName { get; set; }

        public void ConvertFrom(WorkSubtype workSubtype, WorkSubtypeView workSubtypeView) 
        {
            workSubtypeView.WorkSubtypePK = workSubtype.WorkSubtypePK;
            workSubtypeView.Name = workSubtype.Name;
            workSubtypeView.WorkTypeFK = workSubtype.WorkTypeFK;
            workSubtypeView.Deleted = workSubtype.Deleted;
        }

        public void ConvertTo(WorkSubtypeView workSubtypeView, WorkSubtype workSubtype) 
        {
            workSubtype.WorkSubtypePK = workSubtypeView.WorkSubtypePK;
            workSubtype.Name = workSubtypeView.Name;
            workSubtype.WorkTypeFK = workSubtypeView.WorkTypeFK;
            workSubtype.Deleted = workSubtypeView.Deleted;
        }

        public void BindDDLs(WorkSubtypeView workSubtypeView, ObjectContext db) 
        {
            IWorkTypesRepository workTypesRepository = new WorkTypesRepository(db);
            workSubtypeView.WorkTypes = new SelectList(workTypesRepository.GetValid().ToList(), "WorkTypePK", "Name");
        }

        public static IQueryable<WorkSubtypeView> GetWorkSubtypeView(IQueryable<WorkSubtype> workSubtypeTable, IQueryable<WorkType> workTypeTable) 
        {
            IQueryable<WorkSubtypeView> workSubtypeViewList = (from t1 in workSubtypeTable
                                       join t2 in workTypeTable on t1.WorkTypeFK equals t2.WorkTypePK

                                       select new WorkSubtypeView
                                       {
                                            WorkSubtypePK   = t1.WorkSubtypePK,
                                            Name            = t1.Name,
                                            WorkTypeName    = t2.Name,
                                       }).AsQueryable<WorkSubtypeView>();

            return workSubtypeViewList;
        }
    }
}
