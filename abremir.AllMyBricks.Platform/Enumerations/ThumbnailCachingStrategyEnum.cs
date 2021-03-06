﻿using System.ComponentModel;

namespace abremir.AllMyBricks.Platform.Enumerations
{
    public enum ThumbnailCachingStrategyEnum
    {
        [Description("Never cache thumbnails")]
        NeverCache,
        [Description("Only cache displayed thumbnails")]
        OnlyCacheDisplayedThumbnails,
        [Description("Cache all thumbnails when synchronizing")]
        CacheAllThumbnailsWhenSynchronizing
    }
}
