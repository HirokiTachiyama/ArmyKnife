using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using stDokuWiki;
using stDokuWiki.AuthManager;
using stDokuWiki.Connector;
using stDokuWiki.Data;


// https://github.com/PetersSharp/stCoCServer/tree/master/stCoCServer/stExtLib/stDokuWikiConnector-dll/Doc

namespace ArmyKnife {

    /// <summary>
    /// DokuWikiに対してログイン、ページ指定によるコンテンツ取得
    /// GET, PUT時に改行コードの変換を実施
    /// </summary>
    class DokuwikiPageManager
    {

        RpcXml rpcXml;
        public DokuwikiPageManager(string url, string user, string pass)
        {

            rpcXml = new RpcXml(url, user, pass);

            // public: 以下を取得するサンプル            
            // XMLMethodPageList dokuList = rpcXml.DokuPageList("public:") as XMLMethodPageList;
            // foreach(var items in dokuList.Params.Param.Value.Array.Data.Value)
            // {
            //     foreach(var item in items.Struct.Member)
            //     {
            //         Console.WriteLine(item.Name + " " + item.Value.String);
            //     }
            // }

        }

        public string GetPageStr(string _pageName)
        {
            string str = rpcXml.DokuPageGet(_pageName);

            // 改行コードの変換
            // 取得直後のLinux改行コード(LF:\n)をWindows方式(CRLF:\r\n)に変換する
            // もしWindowsのCRLFが混ざっていたら \r\r\n となるので、
            // \r\r があったら \r と変換する処理も念の為行う
            // Windows:CR+LF(\r\n), Linux:LF(\n)
            str = str.Replace("\n", "\r\n").Replace("\r\r", "\r");

            return str;
        }

        public bool PutPage(string _contentStr, string _pageName)
        {
            bool ret = false;

            // Windows改行(CRLF) -> Linux改行(LF)
            string str = _contentStr.Replace("\r\n", "\n");

            try {
                ret = rpcXml.DokuPagePut(_pageName, str);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return ret;
        }
    }
}