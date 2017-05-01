using System;
using System.Collections.Generic;
using System.Text;
using SearchEverything.EverythingApi.Exceptions;

namespace SearchEverything.EverythingApi
{
    public sealed class EverythingApiManager
    {
        private readonly TextWidthManager _textWidthManager = new TextWidthManager();

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
        /// <param name="width">Possible width for showing path.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="maxCount">The max count.</param>
        /// <returns></returns>
        public List<SearchResult> Search(string keyWord, SearchBoxInfo searchBoxInfo, int offset = 0, int maxCount = 100)
        {
            if (string.IsNullOrEmpty(keyWord))
                throw new ArgumentNullException(nameof(keyWord));

            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset));

            if (maxCount < 0)
                throw new ArgumentOutOfRangeException(nameof(maxCount));

            EverythingNativeApi.Everything_SetSearchW((IncludeFolders ? "" : "file:") + keyWord);
            EverythingNativeApi.Everything_SetOffset(offset);
            EverythingNativeApi.Everything_SetMax(maxCount);

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

            const int bufferSize = 4096;
            StringBuilder buffer = new StringBuilder(bufferSize);
            var everythingGetNumResults = EverythingNativeApi.Everything_GetNumResults();
            var resultList = new List<SearchResult>(everythingGetNumResults);
            for (int idx = 0; idx < everythingGetNumResults; ++idx)
            {
                EverythingNativeApi.Everything_GetResultFullPathNameW(idx, buffer, bufferSize);

                var fullPath = buffer.ToString();
                var result = new SearchResult
                {
                    FullPath = fullPath,
                    ShowPath = _textWidthManager.GetSubStringForWidth(fullPath, searchBoxInfo)
                };

                if (EverythingNativeApi.Everything_IsFolderResult(idx))
                {
                    result.Type = ResultType.Folder;
                }
                else if (EverythingNativeApi.Everything_IsFileResult(idx))
                {
                    result.Type = ResultType.File;
                }
                result.ImageSource = IconManager.GetImageSource(result.FullPath, result.Type);
                resultList.Add(result);
            }
            return resultList;
        }
    }
}
