namespace JanHafner.Toolkit.Windows
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Security.Permissions;
    using JetBrains.Annotations;

    [Serializable]
    public sealed class ExecutableNotFoundException : FileNotFoundException
    {
        public ExecutableNotFoundException([NotNull] String file)
            : base($"The executable for file '{file}' was not found.")
        {
            if (String.IsNullOrEmpty(file))
            {
                throw new ArgumentNullException(nameof(file));    
            }

            this.File = file;
        }

        private ExecutableNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        [NotNull]
        public String File { get; private set; }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue("File", this.File);

            base.GetObjectData(info, context);
        }
    }
}
