using IntelliTect.Coalesce.Controllers;
using IntelliTect.Coalesce.Data;
using IntelliTect.Coalesce.Mapping;
using IntelliTect.Coalesce.Models;
using IntelliTect.Coalesce.TypeDefinition;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using Coalesce.Web.Models;
using Coalesce.Domain;
using Coalesce.Domain.External;

namespace Coalesce.Web.Api
{
    [Route("api/[controller]")]
    [Authorize]
    public partial class CaseProductController 
         : LocalBaseApiController<CaseProduct, CaseProductDtoGen> 
    {
        private ClassViewModel _model;

        public CaseProductController() 
        { 
             _model = ReflectionRepository.Models.Single(m => m.Name == "CaseProduct");
        }
      

        /// <summary>
        /// Returns CaseProductDtoGen
        /// </summary>
        [HttpGet("list")]
        [Authorize]
        public virtual async Task<GenericListResult<CaseProduct, CaseProductDtoGen>> List(
            string includes = null, 
            string orderBy = null, string orderByDescending = null,
            int? page = null, int? pageSize = null, 
            string where = null, 
            string listDataSource = null, 
            string search = null, 
            // Custom fields for this object.
            string caseProductId = null,string caseId = null,string productId = null)
        {
            
            ListParameters parameters = new ListParameters(null, includes, orderBy, orderByDescending, page, pageSize, where, listDataSource, search);

            // Add custom filters
            parameters.AddFilter("CaseProductId", caseProductId);
            parameters.AddFilter("CaseId", caseId);
            parameters.AddFilter("ProductId", productId);
        
            var listResult = await ListImplementation(parameters);
            return new GenericListResult<CaseProduct, CaseProductDtoGen>(listResult);
        }

        /// <summary>
        /// Returns custom object based on supplied fields
        /// </summary>
        [HttpGet("customlist")]
        [Authorize]
        public virtual async Task<ListResult> CustomList(
            string fields = null, 
            string includes = null, 
            string orderBy = null, string orderByDescending = null,
            int? page = null, int? pageSize = null, 
            string where = null, 
            string listDataSource = null, 
            string search = null, 
            // Custom fields for this object.
            string caseProductId = null,string caseId = null,string productId = null)
        {

            ListParameters parameters = new ListParameters(fields, includes, orderBy, orderByDescending, page, pageSize, where, listDataSource, search);

            // Add custom filters
            parameters.AddFilter("CaseProductId", caseProductId);
            parameters.AddFilter("CaseId", caseId);
            parameters.AddFilter("ProductId", productId);
        
            return await ListImplementation(parameters);
        }

        [HttpGet("count")]
        [Authorize]
        public virtual async Task<int> Count(
            string where = null, 
            string listDataSource = null,
            string search = null,
            // Custom fields for this object.
            string caseProductId = null,string caseId = null,string productId = null)
        {
            
            ListParameters parameters = new ListParameters(where: where, listDataSource: listDataSource, search: search, fields: null);

            // Add custom filters
            parameters.AddFilter("CaseProductId", caseProductId);
            parameters.AddFilter("CaseId", caseId);
            parameters.AddFilter("ProductId", productId);
            
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
        public virtual async Task<CaseProductDtoGen> Get(string id, string includes = null)
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
        public virtual SaveResult<CaseProductDtoGen> Save(CaseProductDtoGen dto, string includes = null, bool returnObject = true)
        {
            
            // Check if creates/edits aren't allowed
            
            if (!dto.CaseProductId.HasValue && !_model.SecurityInfo.IsCreateAllowed(User)) {
                var result = new SaveResult<CaseProductDtoGen>();
                result.WasSuccessful = false;
                result.Message = "Create not allowed on CaseProduct objects.";
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return result;
            }
            else if (dto.CaseProductId.HasValue && !_model.SecurityInfo.IsEditAllowed(User)) {
                var result = new SaveResult<CaseProductDtoGen>();
                result.WasSuccessful = false;
                result.Message = "Edit not allowed on CaseProduct objects.";
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return result;
            }

            return SaveImplementation(dto, includes, returnObject);
        }
        
        [HttpPost("AddToCollection")]
        [Authorize]
        public virtual SaveResult<CaseProductDtoGen> AddToCollection(int id, string propertyName, int childId)
        {
            return ChangeCollection(id, propertyName, childId, "Add");
        }
        [HttpPost("RemoveFromCollection")]
        [Authorize]
        public virtual SaveResult<CaseProductDtoGen> RemoveFromCollection(int id, string propertyName, int childId)
        {
            return ChangeCollection(id, propertyName, childId, "Remove");
        }
        
        [Authorize]
        protected override IQueryable<CaseProduct> GetListDataSource(ListParameters parameters)
        {

            return base.GetListDataSource(parameters);
        }

        // Methods
    }
}
