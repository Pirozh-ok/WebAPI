﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Habr.Common.Resources {
    using System;
    
    
    /// <summary>
    ///   Класс ресурса со строгой типизацией для поиска локализованных строк и т.д.
    /// </summary>
    // Этот класс создан автоматически классом StronglyTypedResourceBuilder
    // с помощью такого средства, как ResGen или Visual Studio.
    // Чтобы добавить или удалить член, измените файл .ResX и снова запустите ResGen
    // с параметром /str или перестройте свой проект VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class PostExceptionMessageResource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public PostExceptionMessageResource() {
        }
        
        /// <summary>
        ///   Возвращает кэшированный экземпляр ResourceManager, использованный этим классом.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Habr.Common.Resources.PostExceptionMessageResource", typeof(PostExceptionMessageResource).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Перезаписывает свойство CurrentUICulture текущего потока для всех
        ///   обращений к ресурсу с помощью этого класса ресурса со строгой типизацией.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на A post with comments cannot be sent to drafts!.
        /// </summary>
        public static string CannotSendDrafts {
            get {
                return ResourceManager.GetString("CannotSendDrafts", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Post text is required!.
        /// </summary>
        public static string EmptyPostText {
            get {
                return ResourceManager.GetString("EmptyPostText", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Post text cannot exceed 2000 characters!.
        /// </summary>
        public static string MaxLengthTextPostExceeded {
            get {
                return ResourceManager.GetString("MaxLengthTextPostExceeded", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Post title cannot exceed 200 characters!.
        /// </summary>
        public static string MaxLengthTitlePostExceeded {
            get {
                return ResourceManager.GetString("MaxLengthTitlePostExceeded", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Post is published!.
        /// </summary>
        public static string PostAlreadyPublished {
            get {
                return ResourceManager.GetString("PostAlreadyPublished", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Published post cannot be edited!.
        /// </summary>
        public static string PostCannotBeEdited {
            get {
                return ResourceManager.GetString("PostCannotBeEdited", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Post not found!.
        /// </summary>
        public static string PostNotFound {
            get {
                return ResourceManager.GetString("PostNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Post not published!.
        /// </summary>
        public static string PostNotPublished {
            get {
                return ResourceManager.GetString("PostNotPublished", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на The post title is required!.
        /// </summary>
        public static string PostTitleRequired {
            get {
                return ResourceManager.GetString("PostTitleRequired", resourceCulture);
            }
        }
    }
}