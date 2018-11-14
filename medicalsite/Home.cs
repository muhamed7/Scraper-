using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace medicalsite
{
    public class Home
    {
        private MedicalDBEntities db = new MedicalDBEntities();

        public bool insertChabter(List<Chabter1_Tbl> obj)
        {
            try
            {
                db.Chabter1_Tbl.AddRange(obj);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
            
        }

        public bool insertSection(List<Section_Tbl> obj)
        {
            try
            {
                db.Section_Tbl.AddRange(obj);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }

        }
        public bool insertdises(dises_Description_tbl obj)
        {
            try
            {
                if (!string.IsNullOrEmpty(obj.code_dec))
                {
                    db.dises_Description_tbl.Add(obj);
                    db.SaveChanges(); 
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }

        }
        public bool insertDiseases(List <Diseases_Tbl> obj)
        {
            try
            {
                db.Diseases_Tbl.AddRange(obj);
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }
        public bool insertDiseases_destails(List< Diseases_destails_tbl> obj)
        {
            try
            {
                db.Diseases_destails_tbl.AddRange(obj);
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }
          public bool insertnotse(List <Notes_Tbl> obj)
        {
            try
            {
                db.Notes_Tbl.AddRange(obj);
                db.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
                throw;
            }
        }

        public bool insertnotse1(List <Notes_Tbl> obj)
        {
            try
            {
                db.Notes_Tbl.AddRange(obj);
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        internal bool insertChabter(List<Section_Tbl> obj)
        {
            throw new NotImplementedException();
        }
    }
}
    

