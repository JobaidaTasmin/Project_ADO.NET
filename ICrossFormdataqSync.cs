using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComProductsProject
{
    public interface ICrossFormdataqSync
    {
        void ReloadData(List<Company> company);
        void UpdateCompany(Company company);
        void RemoveCompany(int id);
    }
}
