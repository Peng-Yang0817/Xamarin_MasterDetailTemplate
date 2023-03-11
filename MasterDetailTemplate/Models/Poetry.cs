using System;
using System.Collections.Generic;
using System.Text;

namespace MasterDetailTemplate.Models
{
    /// <summary>
    /// 詩詞類
    /// </summary>
    [SQLite.Table(name: "works")]
    public class Poetry
    {
        /// <summary>
        /// 主鍵。
        /// [  ]中的寫法代表Id映射到Table中的id
        /// </summary>
        [SQLite.Column("id")]
        public int Id { get; set; }

        [SQLite.Column("name")]
        public string Name { get; set; }

        [SQLite.Column("author_name")]
        public string AuthorName { get; set; }

        [SQLite.Column("dynasty")]
        public string Dynasty { get; set; }

        [SQLite.Column("content")]
        public string Content { get; set; }

        [SQLite.Column("translation")]
        public string Translation { get; set; }

        /// <summary>
        /// 怕字符串寫錯，因此可以用下面方式來寫死值的選擇
        /// </summary>
        [SQLite.Column("layout")]
        public string Layout { get; set; }

        /// <summary>
        /// 這樣之後在對Latout附值時就可以直接寫
        /// forech(var p in Poetrys)
        /// {
        ///     p.Latout = Poetry.CenterLayout;
        /// }
        /// </summary>
        public const string CenterLayout = "center";
        public const string IndentLayout = "indent";

        /// <summary>
        /// 預覽，存取變量
        /// </summary>
        private string _snippet;

        /// <summary>
        /// 透過第一句話當作預覽的文字。
        /// </summary>
        [SQLite.Ignore]
        public string Snippet =>
            _snippet ?? (_snippet = Content
                    .Split('。')[0]
                    .Replace("\r\n", " "));
    }
}
