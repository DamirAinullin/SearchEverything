using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using SearchEverything.EverythingApi.Exceptions;
using SearchEverything.Icons;
using SearchEverything.Search;
using SearchEverything.Utilities;

namespace SearchEverything.EverythingApi
{
    public sealed class EverythingApiManager
    {
        private const int BufferSize = 4096;
        private readonly TextWidthManager _textWidthManager = new TextWidthManager();
        private static int _maxCount;

        /// <summary>
        /// Searches the specified key word.
        /// </summary>
        /// <param name="keyWord">The key word.</param>
        /// <param name="token"></param>
        /// <param name="maxCount">The max count.</param>
        /// <param name="searchBoxInfo"></param>
        /// <returns></returns>
        public List<SearchResult> Search(string keyWord, SearchBoxInfo searchBoxInfo, CancellationToken token, int maxCount)
        {
            EverythingNativeApi.Everything_SetSearchW(keyWord);
            if (_maxCount != maxCount)
            {
                EverythingNativeApi.Everything_SetMax(maxCount);
                _maxCount = maxCount;
            }

            token.ThrowIfCancellationRequested();

            if (!EverythingNativeApi.Everything_QueryW(true))
            {
                GetLastError();
            }

            token.ThrowIfCancellationRequested();

            var buffer = new StringBuilder(BufferSize);
            var everythingGetNumResults = EverythingNativeApi.Everything_GetNumResults();
            var resultList = new List<SearchResult>(everythingGetNumResults);

            for (int idx = 0; idx < everythingGetNumResults; ++idx)
            {
                EverythingNativeApi.Everything_GetResultFullPathNameW(idx, buffer, BufferSize);

                var fullPath = buffer.ToString();
                var result = new SearchResult
                {
                    FullPath = fullPath,
                    ShowPath = _textWidthManager.GetSubStringForWidth(fullPath, searchBoxInfo),
                    Type = EverythingNativeApi.Everything_IsFolderResult(idx)
                        ? ResultType.Folder
                        : ResultType.File
                };

                result.ImageSource = IconManager.GetImageSource(result.FullPath, result.Type);
                
                resultList.Add(result);

                token.ThrowIfCancellationRequested();
            }
            return resultList.OrderBy(item => item.Type).ToList();
        }

        private void GetLastError()
        {
            switch (EverythingNativeApi.Everything_GetLastError())
            {
                case StateCode.CreateThreadError:
                    throw new CreateThreadException();
                case StateCode.CreateWindowError:
                    throw new CreateWindowException();
                case StateCode.InvalidCallError:
                    throw new InvalidCallException();
                case StateCode.InvalidIndexError:
                    throw new InvalidIndexException();
                case StateCode.IPCError:
                    throw new IPCErrorException();
                case StateCode.MemoryError:
                    throw new MemoryErrorException();
                case StateCode.RegisterClassExError:
                    throw new RegisterClassExException();
            }
        }
    }
}
