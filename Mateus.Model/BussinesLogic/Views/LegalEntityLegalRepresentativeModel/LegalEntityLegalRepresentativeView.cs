using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Mateus.Model.EFModel;
using System.Web.Mvc;
using Mateus.Model.EFModel.Repository.Interface;
using Mateus.Model.EFModel.Repository.Concrete;
using System.Data.Objects;
using Mateus.Model.BussinesLogic.Views.PhysicalEntityModel;

namespace Mateus.Model.BussinesLogic.Views.LegalEntityLegalRepresentativeModel
{
    public class LegalEntityLegalRepresentativeView
    {
        public int LegalEntityLegalRepresentativePK { get; set; }

        [Required(ErrorMessage = "Tvrtka je obavezna.")]
        public int? LegalEntityFK { get; set; }

        [Required(ErrorMessage = "Način zastupanja je obavezan.")]
        public int? WayOfRepresentationFK { get; set; }

        [Required(ErrorMessage = "Pravni zastupnik je obavezan.")]
        public int? LegalRepresentativeFK { get; set; }

        [Required(ErrorMessage = "Datum rješenja je obavezan.")]
        public DateTime? ChangeDate { get; set; }

        public bool? Deleted { get; set; }

        public IEnumerable<SelectListItem> LegalEntities { get; set; }
        public IEnumerable<SelectListItem> WaysOfRepresentation { get; set; }
        public IEnumerable<SelectListItem> LegalRepresentatives { get; set; }

        public string LegalEntityName { get; set; }
        public string LegalRepresentativeName { get; set; }

        public void ConvertFrom(LegalEntityLegalRepresentative legalEntityLegalRepresentative, LegalEntityLegalRepresentativeView legalEntityLegalRepresentativeView, ObjectContext db) 
        {
            legalEntityLegalRepresentativeView.LegalEntityLegalRepresentativePK = legalEntityLegalRepresentative.LegalEntityLegalRepresentativePK;

            legalEntityLegalRepresentativeView.LegalEntityFK                    = legalEntityLegalRepresentative.LegalEntityFK;
            legalEntityLegalRepresentativeView.WayOfRepresentationFK            = legalEntityLegalRepresentative.WayOfRepresentationFK;
            legalEntityLegalRepresentativeView.LegalRepresentativeFK            = legalEntityLegalRepresentative.LegalRepresentativeFK;

            legalEntityLegalRepresentativeView.Deleted                          = legalEntityLegalRepresentative.Deleted;

            ILegalEntitiesRepository legalEntitiesRepository = new LegalEntitiesRepository(db);
            LegalEntity legalEntity = legalEntitiesRepository.GetLegalEntityByPK((int)legalEntityLegalRepresentative.LegalEntityFK);
            legalEntityLegalRepresentativeView.LegalEntityName = legalEntity.Name + " (" + legalEntity.OIB + ")";
        }

        public void ConvertTo(LegalEntityLegalRepresentativeView legalEntityLegalRepresentativeView, LegalEntityLegalRepresentative legalEntityLegalRepresentative) 
        {
            legalEntityLegalRepresentative.LegalEntityFK            = legalEntityLegalRepresentativeView.LegalEntityFK;
            legalEntityLegalRepresentative.WayOfRepresentationFK    = legalEntityLegalRepresentativeView.WayOfRepresentationFK;
            legalEntityLegalRepresentative.LegalRepresentativeFK    = legalEntityLegalRepresentativeView.LegalRepresentativeFK;

            legalEntityLegalRepresentative.ChangeDate             = legalEntityLegalRepresentativeView.ChangeDate;

            legalEntityLegalRepresentative.Deleted                  = legalEntityLegalRepresentativeView.Deleted;
        }

        public void BindDDLs(LegalEntityLegalRepresentativeView legalEntityLegalRepresentativeView, ObjectContext db) 
        {
            //way of representation ddl
            IWaysOfRepresentationRepository wayOfRepresentationView = new WaysOfRepresentationRepository(db);
            legalEntityLegalRepresentativeView.WaysOfRepresentation = new SelectList(wayOfRepresentationView.GetValid().ToList(), "WayOfRepresentationPK", "Name");

            //physicalEntity ddl
            PhysicalEntityView PhysicalEntityView = new PhysicalEntityView();
            IPhysicalEntitiesRepository physicalEntitiesRepository = new PhysicalEntitiesRepository(db);
            legalEntityLegalRepresentativeView.LegalRepresentatives = new SelectList(PhysicalEntityView.GetPhysicalEntitySelect(physicalEntitiesRepository.GetValid()).ToList(), "PhysicalEntityPK", "Name");

        }

        public static IQueryable<LegalEntityLegalRepresentativeView> GetLegalEntityLegalRepresentativeView(IQueryable<LegalEntityLegalRepresentative> legalEntityLegalRepresentativeTable, IQueryable<LegalEntity> legalEntityTable, IQueryable<PhysicalEntity> physicalEntityTable) 
        {
            IQueryable<LegalEntityLegalRepresentativeView> legalEntityLegalRepresentativeViewList = (from t1 in legalEntityLegalRepresentativeTable
                                       join t2 in legalEntityTable on t1.LegalEntityFK equals t2.LegalEntityPK
                                       join t3 in physicalEntityTable on t1.LegalRepresentativeFK equals t3.PhysicalEntityPK

                                       select new LegalEntityLegalRepresentativeView
                                       {
                                            LegalEntityLegalRepresentativePK    = t1.LegalEntityLegalRepresentativePK,
                                            LegalEntityName                     = t2.Name,
                                            LegalEntityFK                       = t1.LegalEntityFK,
                                            LegalRepresentativeName             = t3.Firstname + " " + t3.Lastname,
                                            LegalRepresentativeFK               = t1.LegalRepresentativeFK
                                       }).AsQueryable<LegalEntityLegalRepresentativeView>();

            return legalEntityLegalRepresentativeViewList;
        }
    }
}
