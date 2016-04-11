
using Intellitect.ComponentModel.Controllers;
using Microsoft.AspNet.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Authorization;
using System.Collections.Generic;
using Intellitect.ComponentModel.Models;
using System.Threading.Tasks;
using System;
using System.Linq;
using Intellitect.ComponentModel.Data;
using Intellitect.ComponentModel.Mapping;
// Model Namespaces 
using Coalesce.Domain;
using Coalesce.Domain.External;

namespace Coalesce.Web.Api
{
    [Route("api/[controller]")]
    [Authorize]
    public partial class CompanyController 
         : LocalBaseApiController<Company> 
    {
        public CompanyController() { }
        
        
        [HttpGet("list")]
        [Authorize]
        public virtual async Task<ListResult> List(
            string fields = null, 
            string include = null, 
            string includes = null, 
            string orderBy = null, string orderByDescending = null,
            int? page = null, int? pageSize = null, 
            string where = null, 
            string listDataSource = null, 
            string search = null, 
            // Custom fields for this object.
            string companyId = null,string name = null,string address1 = null,string address2 = null,string city = null,string state = null,string zipCode = null,string altName = null)
        {
            ListParameters parameters = new ListParameters(fields, include, includes, orderBy, orderByDescending, page, pageSize, where, listDataSource, search);

            // Add custom filters
            parameters.AddFilter("CompanyId", companyId);
            parameters.AddFilter("Name", name);
            parameters.AddFilter("Address1", address1);
            parameters.AddFilter("Address2", address2);
            parameters.AddFilter("City", city);
            parameters.AddFilter("State", state);
            parameters.AddFilter("ZipCode", zipCode);
            parameters.AddFilter("AltName", altName);
        
            return await ListImplementation(parameters);
        }


        [HttpGet("count")]
        [Authorize]
        public virtual async Task<int> Count(
            string where = null, 
            string listDataSource = null,
            string search = null,
            // Custom fields for this object.
            string companyId = null,string name = null,string address1 = null,string address2 = null,string city = null,string state = null,string zipCode = null,string altName = null)
        {
            ListParameters parameters = new ListParameters(where: where, listDataSource: listDataSource, search: search);

            // Add custom filters
            parameters.AddFilter("CompanyId", companyId);
            parameters.AddFilter("Name", name);
            parameters.AddFilter("Address1", address1);
            parameters.AddFilter("Address2", address2);
            parameters.AddFilter("City", city);
            parameters.AddFilter("State", state);
            parameters.AddFilter("ZipCode", zipCode);
            parameters.AddFilter("AltName", altName);
            
            return await CountImplementation(parameters);
        }

        [HttpGet("propertyValues")]
        [Authorize]
        public virtual IEnumerable<string> PropertyValues(string property, int page = 1, string search = "")
        {
            return PropertyValuesImplementation(property, page, search);
        }

        [HttpGet("get/{id}")]
        [Authorize]
        public virtual async Task<Company> Get(string id, string includes = null)
        {
            return await GetImplementation(id, includes);
        }


        [HttpPost("delete/{id}")]
        [Authorize]
        public virtual bool Delete(string id)
        {
            return DeleteImplementation(id);
        }

        [HttpPost("save")]
        [Authorize]
        public virtual SaveResult<Company> Save(Company dto, string includes = null, bool returnObject = true)
        {
            return SaveImplementation(dto, includes, returnObject);
        }

        [HttpPost("AddToCollection")]
        [Authorize]
        public virtual SaveResult<Company> AddToCollection(int id, string propertyName, int childId)
        {
            return ChangeCollection(id, propertyName, childId, "Add");
        }
        [HttpPost("RemoveFromCollection")]
        [Authorize]
        public virtual SaveResult<Company> RemoveFromCollection(int id, string propertyName, int childId)
        {
            return ChangeCollection(id, propertyName, childId, "Remove");
        }
        

        protected override IQueryable<Company> GetListDataSource(ListParameters parameters)
        {

            return base.GetListDataSource(parameters);
        }

        // Methods
    }
}