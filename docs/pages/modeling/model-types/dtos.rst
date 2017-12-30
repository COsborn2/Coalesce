.. _CustomDTOs:

Custom DTOs
===========

In addition to the generated :ref:`DTOs` that Coalesce will create for you, you may also create your own implementations of an :csharp:`IClassDto`. These types are first-class citizens in Coalesce - you will get a full suite of features surrounding them as if they were entities. This includes generated API Controllers, Admin Views, and full :ref:`TypeScriptViewModel` and :ref:`TypeScriptListViewModel`.

The difference between a Custom DTO and the underlying entity that they represent is as follows:

    - The only time your custom DTO will be served is when it is requested directly from one of the endpoints on its generated controller. It will not be used when making a call to an API endpoint that was generated from an entity.

    - When mapping data from your database, or unmapping data from the client, the DTO itself must manually map all properties, since there is no corresponding :ref:`Generated DTO <GenDTOs>`. Attributes like :ref:`DtoIncludesExcludesAttr` and property-level security through :ref:`SecurityAttributes` have no effect on custom DTOs, since those attribute only affect what get generated for :ref:`GenDTOs`.


Creating a Custom DTO
---------------------

To create a custom DTO, define a class that implements :csharp:`IClassDTO<T>`, where :csharp:`T` is an EF Core POCO, and annotate it with :csharp:`[Coalesce]` . Add any :ref:`ModelProperties` to it just as you would add :ref:`model properties <ModelProperties>` to a regular EF model.

Next, ensure that one property is annoted with :csharp:`[Key]` so that Coalesce can know the primary key of your DTO in order to perform database lookups and keep track of your object uniquely in the client-side TypeScript.

Now, populate the required :csharp:`MapTo` and :csharp:`MapFrom` methods with code for mapping from and to your DTO, respectively (the methods are named with respect to the underlying entity, not the DTO). Most properties probably map one-to-one in both directions, but you probably created a DTO because you wanted some sort of custom mapping - say, mapping a collection on your entity with a comma-delimited string on the DTO. This is also the place to perform any user-based, role-based, property-level security. You can access the current user on the :csharp:`IMappingContext` object. 

.. code-block:: c#

    [Coalesce]
    public class CaseDto : IClassDto<Case>
    {
        [Key]
        public int CaseId { get; set; }

        public string Title { get; set; }

        [Read]
        public string AssignedToName { get; set; }

        public void MapTo(Case obj, IMappingContext context)
        {
            obj.Title = Title;
        }

        public void MapFrom(Case obj, IMappingContext context = null, IncludeTree tree = null)
        {
            CaseId = obj.CaseKey;
            Title = obj.Title;
            if (obj.AssignedTo != null)
            {
                AssignedToName = obj.AssignedTo.Name;
            }
        }
    }

.. warning::

    Custom DTOs do not utilize property-level :ref:`SecurityAttributes` nor :ref:`DtoIncludesExcludesAttr`, since these are handled in the :ref:`Generated DTOs <GenDTOs>`. If you need property-level security or trimming, you must write it yourself in the :csharp:`MapTo` and :csharp:`MapFrom` methods.

If you have any child objects on your DTO, you can invoke the mapper for some other object using the static :csharp:`Mapper` class. Also seen in this example is how to respect the :ref:`IncludeTree` when mapping entity types; however, respecting the :csharp:`IncludeTree` is optional. Since this DTO is a custom type that you've written, if you're certain your use cases don't need to worry about object graph trimming, then you can ignore the :csharp:`IncludeTree`. If you do ignore the :csharp:`IncludeTree`, you should pass :csharp:`null` to calls to :csharp:`Mapper` - don't pass in the incoming :csharp:`IncludeTree`, as this could cause unexpected results.

.. code-block:: c#

    using IntelliTect.Coalesce.Mapping;

    [Coalesce]
    public class CaseDto : IClassDto<Case>
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }
        ...

        public void MapFrom(Case obj, IMappingContext context = null, IncludeTree tree = null)
        {
            ProductId = obj.ProductId;

            if (tree == null || tree[nameof(this.Product)] != null)
                Product = Mapper.MapToDto<Product, ProductDtoGen>(obj.Product, context, tree?[nameof(this.Product)]
            ...
        }
    }