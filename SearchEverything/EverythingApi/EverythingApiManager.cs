using System.Collections.Generic;
using System.Text;
using System.Threading;
using SearchEverything.EverythingApi.Exceptions;

namespace SearchEverything.EverythingApi
{
    public sealed class EverythingApiManager
    {
        private const int BufferSize = 4096;
        private readonly TextWidthManager _textWidthManager = new TextWidthManager();
        private static int _maxCount;
        /// <summary>
        /// Gets or sets a value indicating whether [match path].
        /// </summary>
        /// <value><c>true</c> if [match path]; otherwise, <c>false</c>.</value>
        public bool MatchPath
        {
            get
            {
                return EverythingNativeApi.Everything_GetMatchPath();
            }
            set
            {
                EverythingNativeApi.Everything_SetMatchPath(value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [match case].
        /// </summary>
        /// <value><c>true</c> if [match case]; otherwise, <c>false</c>.</value>
        public bool MatchCase
        {
            get
            {
                return EverythingNativeApi.Everything_GetMatchCase();
            }
            set
            {
                EverythingNativeApi.Everything_SetMatchCase(value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [match whole word].
        /// </summary>
        /// <value><c>true</c> if [match whole word]; otherwise, <c>false</c>.</value>
        public bool MatchWholeWord
        {
            get
            {
                return EverythingNativeApi.Everything_GetMatchWholeWord();
            }
            set
            {
                EverythingNativeApi.Everything_SetMatchWholeWord(value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [enable regex].
        /// </summary>
        /// <value><c>true</c> if [enable regex]; otherwise, <c>false</c>.</value>
        public bool EnableRegex
        {
            get
            {
                return EverythingNativeApi.Everything_GetRegex();
            }
            set
            {
                EverythingNativeApi.Everything_SetRegex(value);
            }
        }

        public bool IncludeFolders { get; set; }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            EverythingNativeApi.Everything_Reset();
        }

        /// <summary>
        /// Searches the specified key word.
        /// </summary>
        /// <param name="keyWord">The key word.</param>
        /// <param name="token"></param>
        /// <param name="maxCount">The max count.</param>
        /// <param name="searchBoxInfo"></param>
        /// <returns></returns>
        public List<SearchResult> Search(string keyWord, SearchBoxInfo searchBoxInfo, CancellationToken token, int maxCount = 100)
        {
            EverythingNativeApi.Everything_SetSearchW((IncludeFolders ? "" : "file:") + keyWord);
            if (_maxCount != maxCount)
            {
                EverythingNativeApi.Everything_SetMax(maxCount);
                _maxCount = maxCount;
            }

            token.ThrowIfCancellationRequested();

            if (!EverythingNativeApi.Everything_QueryW(true))
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
                    ShowPath = _textWidthManager.GetSubStringForWidth(fullPath, searchBoxInfo)
                };
                result.ImageSource = IconManager.GetImageSource(result.FullPath, result.Type);
                if (IncludeFolders)
                {
                    result.Type = EverythingNativeApi.Everything_IsFolderResult(idx)
                        ? ResultType.Folder
                        : ResultType.File;
                }
                resultList.Add(result);

                token.ThrowIfCancellationRequested();
            }
            return resultList;
        }
    }
}
