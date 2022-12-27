using System.Collections.Generic;
using System.Data;
using Vestcom.PaperSign.ACME.Entities;

namespace Vestcom.PaperSign.ACME.DataAccessLayer.Converters
{
    /// <summary>
    /// IConverter interface
    /// </summary>
    public interface IConverter
    {
        /// <summary>
        /// Converts the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="items">The items.</param>
        void Convert(IDataReader reader, IList<Department> items);
        
        /// <summary>
        /// Converts the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="items">The items.</param>
        void Convert(IDataReader reader, IList<Heading> items);
       
        /// <summary>
        /// Converts the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="items">The items.</param>
        void Convert(IDataReader reader, IList<Image> items);
        
        /// <summary>
        /// Converts the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="items">The items.</param>
        void Convert(IDataReader reader, IList<PromoPGM> items);
        
        /// <summary>
        /// Converts the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="items">The items.</param>
        void Convert(IDataReader reader, IList<UnitPriceEntry> items);
        
        /// <summary>
        /// Converts the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="items">The items.</param>
        void Convert(IDataReader reader, IList<SignLayout> items);
        
        /// <summary>
        /// Converts the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="items">The items.</param>
        void Convert(IDataReader reader, IList<InputFile> items);
        
        /// <summary>
        /// Converts the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="items">The items.</param>
        void Convert(IDataReader reader, IList<AcmeRecord> items); // for Downback
        
        /// <summary>
        /// Converts the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="list">The list.</param>
        void Convert(IDataReader reader, IList<SubstituteStock> list);
        void Convert(IDataReader reader, IList<HolidayInput> items);
    }
}
