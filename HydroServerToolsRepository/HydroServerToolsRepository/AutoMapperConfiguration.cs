using AutoMapper;
using HydroserverToolsBusinessObjects.Models;
using ODM_1_1_1EFModel;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroServerToolsRepository
{
    
    public static class AutoMapperWebConfiguration
    {
        public static void Configure()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile(new UserProfile());
               
            });
        }
    }

    public class UserProfile : Profile
    {
        protected override void Configure()
        {
            
            Mapper.CreateMap<string, int>().ConvertUsing<IntTypeConverter>();
            Mapper.CreateMap<string, int?>().ConvertUsing<NullIntTypeConverter>();
            Mapper.CreateMap<string, decimal?>().ConvertUsing<NullDecimalTypeConverter>();
            Mapper.CreateMap<string, decimal>().ConvertUsing<DecimalTypeConverter>();
            Mapper.CreateMap<string, bool?>().ConvertUsing<NullBooleanTypeConverter>();
            Mapper.CreateMap<string, bool>().ConvertUsing<BooleanTypeConverter>();
            Mapper.CreateMap<string, Int64?>().ConvertUsing<NullInt64TypeConverter>();
            Mapper.CreateMap<string, Int64>().ConvertUsing<Int64TypeConverter>();
            Mapper.CreateMap<string, DateTime?>().ConvertUsing<NullDateTimeTypeConverter>();
            Mapper.CreateMap<string, DateTime>().ConvertUsing<DateTimeTypeConverter>();
            Mapper.CreateMap<string, double?>().ConvertUsing<NullDoubleTypeConverter>();
            Mapper.CreateMap<string, double>().ConvertUsing<DoubleTypeConverter>();

            //  Sites
            Mapper.CreateMap<Site, SiteModel>();
            Mapper.CreateMap<SiteModel, Site>()
                .ForMember(dest => dest.LatLongDatumID, opt => opt.Ignore());
                //ForMember(dest => dest.LatLongDatumID, opt  => opt.ResolveUsing<MyLongLatDatumIdResolver>());
                //     .ForMember(x => x.LatLongDatumID, c => c.MapFrom(src => src ));
                //.ForMember()
             //  Variables
            Mapper.CreateMap<Variable, VariablesModel>();
            Mapper.CreateMap<VariablesModel, Variable>();

            //  OffsetTypes
            Mapper.CreateMap<OffsetType, OffsetTypesModel>();
            Mapper.CreateMap<OffsetTypesModel, OffsetType>();

            //  ISOMetadata
            Mapper.CreateMap<ISOMetadata, ISOMetadataModel>();
            Mapper.CreateMap<ISOMetadataModel, ISOMetadata>();

            //  Sources
            Mapper.CreateMap<Source, SourcesModel>();
            Mapper.CreateMap<SourcesModel, Source>();

            //  Methods
            Mapper.CreateMap<Method, MethodModel>();
            Mapper.CreateMap<MethodModel, Method>();

            //  LabMethods
            Mapper.CreateMap<LabMethod, LabMethodModel>();
            Mapper.CreateMap<LabMethodModel, LabMethod>();

            //  Samples
            Mapper.CreateMap<Sample, SampleModel>();
            Mapper.CreateMap<SampleModel, Sample>();

            //  Qualifiers
            Mapper.CreateMap<Qualifier, QualifiersModel>();
            Mapper.CreateMap<QualifiersModel, Qualifier>();

            //  QualityControlLevels
            Mapper.CreateMap<QualityControlLevel, QualityControlLevelModel>();
            Mapper.CreateMap<QualityControlLevelModel, QualityControlLevel>();

            //  DataValues
            Mapper.CreateMap<DataValue, DataValuesModel>();
            Mapper.CreateMap<DataValuesModel, DataValue>();

            //  GroupDescriptions
            Mapper.CreateMap<GroupDescription, GroupDescriptionModel>();
            Mapper.CreateMap<GroupDescriptionModel, GroupDescription>();

            //  Groups
            Mapper.CreateMap<Group, GroupsModel>();
            Mapper.CreateMap<GroupsModel, Group>();

            //  DerivedFrom
            Mapper.CreateMap<DerivedFrom, DerivedFromModel>();
            Mapper.CreateMap<DerivedFromModel, DerivedFrom>();

            //  Categories
            Mapper.CreateMap<Category, CategoriesModel>();
            Mapper.CreateMap<CategoriesModel, Category>();

        }
    }

    //from here http://pastebin.com/cEMCbRp
    // TODO: Boil down to two with Generics if possible w
    #region AutoMapTypeConverters
    // Automap type converter definitions for 
    // int, int?, decimal, decimal?, bool, bool?, Int64, Int64?, DateTime
    // Automapper string to int?
    public class NullIntTypeConverter : TypeConverter<string, int?>
    {
        protected override int? ConvertCore(string source)
        {
            if (source == null || source.ToLower() == "null")
                return null;
            else
            {
                int result;
                return Int32.TryParse(source, out result) ? (int?)result : null;
            }
        }
    }
    // Automapper string to int
    public class IntTypeConverter : TypeConverter<string, int>
    {
        protected override int ConvertCore(string source)
        {
            if (source == null)
                throw new MappingException("null string value cannot convert to non-nullable return type.");
            else
                return Int32.Parse(source);
        }
    }
    // Automapper string to decimal?
    public class NullDecimalTypeConverter : TypeConverter<string, decimal?>
    {
        protected override decimal? ConvertCore(string source)
        {
            if (source == null || source.ToLower() == "null")
                return null;
            else
            {
                decimal result;
                return Decimal.TryParse(source, out result) ? (decimal?)result : null;
            }
        }
    }
    // Automapper string to decimal
    public class DecimalTypeConverter : TypeConverter<string, decimal>
    {
        protected override decimal ConvertCore(string source)
        {
            if (source == null)
                throw new MappingException("null string value cannot convert to non-nullable return type.");
            else
                return Decimal.Parse(source);
        }
    }
    // Automapper string to bool?
    public class NullBooleanTypeConverter : TypeConverter<string, bool?>
    {
        protected override bool? ConvertCore(string source)
        {
            if (source == null || source.ToLower() == "null")
                return null;
            else
            {
                bool result;
                return Boolean.TryParse(source, out result) ? (bool?)result : null;
            }
        }
    }
    // Automapper string to bool
    public class BooleanTypeConverter : TypeConverter<string, bool>
    {
        protected override bool ConvertCore(string source)
        {
            if (source == null)
                throw new MappingException("null string value cannot convert to non-nullable return type.");
            if (source == "1")
                return true;
            if (source == "0")
                return false;
            else
                return Boolean.Parse(source);
        }
    }
    // Automapper string to Int64?
    public class NullInt64TypeConverter : TypeConverter<string, Int64?>
    {
        protected override Int64? ConvertCore(string source)
        {
            if (source == null || source.ToLower() == "null")
                return null;
            else
            {
                Int64 result;
                return Int64.TryParse(source, out result) ? (Int64?)result : null;
            }
        }
    }
    // Automapper string to Int64
    public class Int64TypeConverter : TypeConverter<string, Int64>
    {
        protected override Int64 ConvertCore(string source)
        {
            if (source == null)
                throw new MappingException("null string value cannot convert to non-nullable return type.");
            else
                return Int64.Parse(source);
        }
    }
    // Automapper string to DateTime?
    // In our case, the datetime will be a JSON2.org datetime
    // Example: "/Date(1288296203190)/"
    public class NullDateTimeTypeConverter : TypeConverter<string, DateTime?>
    {
        protected override DateTime? ConvertCore(string source)
        {
            if (source == null || source.ToLower() == "null")
                return null;
            else
            {
                DateTime result;
                return DateTime.TryParse(source, out result) ? (DateTime?)result : null;
            }
        }
    }
    // Automapper string to DateTime
    public class DateTimeTypeConverter : TypeConverter<string, DateTime>
    {
        protected override DateTime ConvertCore(string source)
        {
            if ((source == null) || (source == string.Empty))
                throw new MappingException("null string value cannot convert to non-nullable return type.");
            else
                return DateTime.Parse(source);
        }
    }
    // Automapper string to Double?
    public class NullDoubleTypeConverter : TypeConverter<string, double?>
    {
        protected override double? ConvertCore(string source)
        {
            if (source == null || source.ToLower() == "null")
                return null;
            else
            {
                double result;
                return double.TryParse(source, out result) ? (double?)result : null;
            }
        }
    }

    public class DoubleTypeConverter : TypeConverter<string, double>
    {
        protected override double ConvertCore(string source)
        {
            if (source == null || source.ToLower() == "null")
                throw new MappingException("null string value cannot convert to non-nullable return type.");
            else
                return double.Parse(source);
        }
    }
    //Automapper string to Bit
    public class BitTypeConverter : TypeConverter<string, int>
    {
        protected override int ConvertCore(string source)
        {
            if (source == null || source.ToLower() == "null")
                throw new MappingException("null string value cannot convert to non-nullable return type.");
            else
                return int.Parse(source);
        }
    }


    #endregion

    public class MyLongLatDatumIdResolver : ValueResolver<SiteModel, string>
    {
        protected override string ResolveCore(SiteModel source)
        {
           // source.LatLongDatumID = 
           
            return  source.LatLongDatumSRSName;
        }
    }
    
}
