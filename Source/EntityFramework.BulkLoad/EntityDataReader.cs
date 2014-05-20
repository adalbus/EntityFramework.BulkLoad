namespace EntityFramework.BulkLoad
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Data.Entity.Core.Mapping;
    using System.Data.Entity.Core.Metadata.Edm;
    using System.Data.Entity.Core.Objects;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    public class EntityDataReader<T> : IDataReader
        where T : class
    {
        #region Fields

        private readonly string[] fieldNames;

        private readonly Type[] fieldTypes;

        private readonly Action<T, object[]> getValuesFunc;

        private readonly object[] values;

        private IEnumerator<T> enumerator;

        #endregion

        #region Constructors and Destructors

        public EntityDataReader(IEnumerable<T> entities, DbContext dbContext)
            : this(entities, ((IObjectContextAdapter)dbContext).ObjectContext)
        {
        }

        public EntityDataReader(IEnumerable<T> entities, ObjectContext objectContext)
        {
            var mapper = new SchemaMapper(objectContext);

            this.fieldNames = mapper.FieldNames;

            this.fieldTypes = mapper.FieldTypes;

            this.getValuesFunc = mapper.GetValuesFunc;

            this.values = new object[this.fieldNames.Length];

            this.enumerator = entities.GetEnumerator();
        }

        #endregion

        #region Public Properties

        public int Depth
        {
            get
            {
                return 0;
            }
        }

        public int FieldCount
        {
            get
            {
                return this.fieldNames.Length;
            }
        }

        public bool IsClosed
        {
            get
            {
                return this.enumerator != null;
            }
        }

        #endregion

        #region Explicit Interface Properties

        int IDataReader.RecordsAffected
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        #endregion

        #region Public Indexers

        public object this[string name]
        {
            get
            {
                return this[this.GetOrdinal(name)];
            }
        }

        public object this[int i]
        {
            get
            {
                return this.values[i];
            }
        }

        #endregion

        #region Public Methods and Operators

        public void Close()
        {
            this.enumerator.Dispose();

            this.enumerator = null;
        }

        public void Dispose()
        {
            this.Close();
        }

        public bool GetBoolean(int i)
        {
            return (bool)this.values[i];
        }

        public byte GetByte(int i)
        {
            return (byte)this.values[i];
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            var bytes = (byte[])this.values[i];

            Array.Copy(bytes, fieldOffset, buffer, bufferoffset, Math.Min(length, bytes.Length - fieldOffset));

            return Math.Min(length, bytes.Length - fieldOffset);
        }

        public char GetChar(int i)
        {
            return (char)this.values[i];
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            var chars = (string)this.values[i];

            chars.CopyTo((int)fieldoffset, buffer, bufferoffset, Math.Min(length, chars.Length - (int)fieldoffset));

            return Math.Min(length, chars.Length - (int)fieldoffset);
        }

        public string GetDataTypeName(int i)
        {
            return this.fieldTypes[i].Name;
        }

        public DateTime GetDateTime(int i)
        {
            return (DateTime)this.values[i];
        }

        public decimal GetDecimal(int i)
        {
            return (decimal)this.values[i];
        }

        public double GetDouble(int i)
        {
            return (double)this.values[i];
        }

        public Type GetFieldType(int i)
        {
            return this.fieldTypes[i];
        }

        public float GetFloat(int i)
        {
            return (float)this.values[i];
        }

        public Guid GetGuid(int i)
        {
            return (Guid)this.values[i];
        }

        public short GetInt16(int i)
        {
            return (short)this.values[i];
        }

        public int GetInt32(int i)
        {
            return (int)this.values[i];
        }

        public long GetInt64(int i)
        {
            return (long)this.values[i];
        }

        public string GetName(int i)
        {
            return this.fieldNames[i];
        }

        public int GetOrdinal(string name)
        {
            return Array.IndexOf(this.fieldNames, name);
        }

        public DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        public string GetString(int i)
        {
            return (string)this.values[i];
        }

        public object GetValue(int i)
        {
            return this.values[i];
        }

        public int GetValues(object[] values)
        {
            Array.Copy(this.values, 0, values, 0, Math.Min(this.values.Length, values.Length));

            return Math.Min(this.values.Length, values.Length);
        }

        public bool IsDBNull(int i)
        {
            return this.values[i] == DBNull.Value;
        }

        public bool NextResult()
        {
            this.Close();

            return false;
        }

        public bool Read()
        {
            bool r = this.enumerator.MoveNext();

            if (r)
            {
                this.getValuesFunc(this.enumerator.Current, this.values);
            }

            else
            {
                for (int i = 0; i < this.values.Length; i++)
                {
                    this.values[i] = null;
                }
            }

            return r;
        }

        #endregion

        #region Explicit Interface Methods

        IDataReader IDataRecord.GetData(int i)
        {
            throw new NotSupportedException();
        }

        #endregion

        private class SchemaMapper
        {
            #region Constructors and Destructors

            public SchemaMapper(ObjectContext objectContext)
            {
                var fieldNamesList = new List<string>();

                var fieldTypesList = new List<Type>();

                this.GetValuesFunc =
                    Reflect.CompileMethod<Action<T, object[]>>(
                        "EntityFramework.BulkLoad.EntityDataReader.getValuesFunc",
                        typeof(EntityDataReader<T>),
                        il =>
                        {
                            BuildSchemaMapping(
                                new List<ComplexPropertyMapping>(),
                                objectContext.MetadataWorkspace.GetItem<EntityContainerMapping>(
                                    objectContext.DefaultContainerName,
                                    DataSpace.CSSpace)
                                    .EntitySetMappings.Single(
                                        s => s.EntitySet.ElementType.FullName == typeof(T).FullName)
                                    .EntityTypeMappings.Single(s => s.EntityType.FullName == typeof(T).FullName)
                                    .Fragments.Single()
                                    .PropertyMappings,
                                fieldNamesList,
                                fieldTypesList,
                                il);

                            il.Emit(OpCodes.Ret);
                        });

                this.FieldNames = fieldNamesList.ToArray();

                this.FieldTypes = fieldTypesList.ToArray();
            }

            #endregion

            #region Public Properties

            public string[] FieldNames { get; private set; }

            public Type[] FieldTypes { get; private set; }

            public Action<T, object[]> GetValuesFunc { get; private set; }

            #endregion

            #region Methods

            private static void BuildSchemaMapping(
                IList<ComplexPropertyMapping> nesting,
                IEnumerable<PropertyMapping> propertyMappings,
                IList<string> fieldNamesList,
                IList<Type> fieldTypesList,
                ILGenerator il)
            {
                foreach (PropertyMapping propertyMapping in propertyMappings)
                {
                    var scalar = propertyMapping as ScalarPropertyMapping;

                    if (scalar != null)
                    {
                        il.Emit(OpCodes.Ldarg_1);

                        il.Emit(OpCodes.Ldc_I4, fieldNamesList.Count);

                        il.Emit(OpCodes.Ldarg_0);

                        Type currentType = typeof(T);

                        foreach (ComplexPropertyMapping level in nesting)
                        {
                            il.Emit(
                                OpCodes.Callvirt,
                                currentType.GetProperty(
                                    level.Property.Name,
                                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                    .GetGetMethod(true));

                            currentType = Type.GetType(level.TypeMappings.Single().ComplexType.FullName);
                        }

                        PropertyInfo endProperty = currentType.GetProperty(
                            scalar.Property.Name,
                            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                        il.Emit(OpCodes.Callvirt, endProperty.GetGetMethod(true));

                        if (endProperty.PropertyType.IsValueType)
                        {
                            il.Emit(OpCodes.Box, endProperty.PropertyType);
                        }

                        il.Emit(OpCodes.Stelem_Ref);

                        fieldNamesList.Add(scalar.Column.Name);

                        fieldTypesList.Add(scalar.Column.PrimitiveType.ClrEquivalentType);

                        continue;
                    }

                    var complex = propertyMapping as ComplexPropertyMapping;

                    if (complex != null)
                    {
                        nesting.Add(complex);

                        BuildSchemaMapping(
                            nesting,
                            complex.TypeMappings.Single().PropertyMappings,
                            fieldNamesList,
                            fieldTypesList,
                            il);

                        nesting.RemoveAt(nesting.Count - 1);

                        continue;
                    }

                    throw new NotImplementedException("Cannot handle a " + propertyMapping.GetType().Name + ".");
                }
            }

            #endregion
        }
    }
}