/* ====================================
   THEME MANAGER
   Handle theme switching and persistence
   ==================================== */

var ThemeManager = (function() {
    'use strict';
    
    // Available themes based on Microsoft Fluent Design
    const THEMES = [
        { id: 'blue', name: 'Blue', color: '#0078d4', icon: 'fa-droplet' },
        { id: 'green', name: 'Green', color: '#107c10', icon: 'fa-leaf' },
        { id: 'teal', name: 'Teal', color: '#008272', icon: 'fa-water' },
        { id: 'purple', name: 'Purple', color: '#5c2d91', icon: 'fa-star' },
        { id: 'magenta', name: 'Magenta', color: '#b4009e', icon: 'fa-heart' },
        { id: 'red', name: 'Red', color: '#d13438', icon: 'fa-fire' },
        { id: 'orange', name: 'Orange', color: '#d97706', icon: 'fa-sun' },
        { id: 'yellow', name: 'Yellow/Gold', color: '#f59e0b', icon: 'fa-star' },
        { id: 'indigo', name: 'Indigo', color: '#4b53bc', icon: 'fa-gem' },
        { id: 'cyan', name: 'Cyan', color: '#00b7c3', icon: 'fa-snowflake' },
        { id: 'navy', name: 'Navy', color: '#002050', icon: 'fa-anchor' },
        { id: 'gray', name: 'Gray', color: '#5d5a58', icon: 'fa-circle' },
        { id: 'lightgray', name: 'Light Gray', color: '#a19f9d', icon: 'fa-circle' },
        { id: 'brown', name: 'Brown', color: '#8e562e', icon: 'fa-tree' },
        { id: 'pink', name: 'Pink', color: '#e3008c', icon: 'fa-heart' }
    ];
    
    const DEFAULT_THEME = 'blue';
    const STORAGE_KEY = 'append-theme';
    
    /**
     * Get all available themes
     */
    function getThemes() {
        return THEMES;
    }
    
    /**
     * Get current active theme
     */
    function getCurrentTheme() {
        return localStorage.getItem(STORAGE_KEY) || DEFAULT_THEME;
    }
    
    /**
     * Set and apply a theme
     */
    function setTheme(themeId) {
        // Validate theme exists
        const theme = THEMES.find(t => t.id === themeId);
        if (!theme) {
            console.warn(`Theme "${themeId}" not found. Using default.`);
            themeId = DEFAULT_THEME;
        }
        
        // Apply theme to document
        document.documentElement.setAttribute('data-theme', themeId);
        
        // Save to localStorage
        localStorage.setItem(STORAGE_KEY, themeId);
        
        // Trigger custom event for other components
        const event = new CustomEvent('themeChanged', { 
            detail: { themeId: themeId, theme: theme } 
        });
        document.dispatchEvent(event);
        
        return theme;
    }
    
    /**
     * Initialize theme system
     */
    function init() {
        const savedTheme = getCurrentTheme();
        setTheme(savedTheme);
    }
    
    /**
     * Get theme by ID
     */
    function getThemeById(themeId) {
        return THEMES.find(t => t.id === themeId) || THEMES[0];
    }
    
    // Public API
    return {
        init: init,
        getThemes: getThemes,
        getCurrentTheme: getCurrentTheme,
        getThemeById: getThemeById,
        setTheme: setTheme,
        DEFAULT_THEME: DEFAULT_THEME
    };
})();

// Auto-initialize when DOM is ready
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', function() {
        ThemeManager.init();
    });
} else {
    ThemeManager.init();
}

// Expose globally
if (typeof window !== 'undefined') {
    window.ThemeManager = ThemeManager;
}
