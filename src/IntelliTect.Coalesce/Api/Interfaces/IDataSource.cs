﻿using System;
using System.Security.Claims;
using System.Threading.Tasks;
using IntelliTect.Coalesce.Mapping.IncludeTree;
using IntelliTect.Coalesce.Interfaces;
using IntelliTect.Coalesce.Models;

namespace IntelliTect.Coalesce
{
    public interface IDataSource<T>
        where T : class, new()
    {
        CrudContext Context { get; }

        Task<(T item, IncludeTree includeTree)> GetItemAsync(object id, IDataSourceParameters parameters);
        Task<TDto> GetMappedItemAsync<TDto>(object id, IDataSourceParameters parameters)
            where TDto : IClassDto<T, TDto>, new();

        Task<(ListResult<T> list, IncludeTree includeTree)> GetListAsync(IListParameters parameters);
        Task<ListResult<TDto>> GetMappedListAsync<TDto>(IListParameters parameters)
            where TDto : IClassDto<T, TDto>, new();

        Task<int> GetCountAsync(IFilterParameters parameters);
    }
}