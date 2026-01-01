<template>
    <div class="theme-picker-container">
        <button class="theme-picker-button" @click="toggleMenu">
            <i class="fa-solid fa-palette"></i>
        </button>
        
        <div v-if="showMenu" class="dropdown-menu dropdown-menu-end show theme-picker-menu p-2" style="min-width: 240px;">
            <div class="theme-grid">
                <div v-for="theme in themes" 
                     :key="theme.id"
                     class="theme-item"
                     :class="{ 'active': currentTheme === theme.id }"
                     @click="selectTheme(theme.id)">
                    <div class="theme-color-preview" :style="{ backgroundColor: theme.color, color: getContrastColor(theme.color) }">
                        <i v-if="currentTheme === theme.id" class="fa-solid fa-check"></i>
                    </div>
                    <div class="theme-name">{{ theme.name }}</div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
export default {
    data() {
        return {
            showMenu: false,
            currentTheme: 'blue',
            themes: [
                { id: 'blue', name: 'Blue', color: '#0078d4' },
                { id: 'green', name: 'Green', color: '#107c10' },
                { id: 'teal', name: 'Teal', color: '#008272' },
                { id: 'purple', name: 'Purple', color: '#5c2d91' },
                { id: 'magenta', name: 'Magenta', color: '#b4009e' },
                { id: 'red', name: 'Red', color: '#d13438' },
                { id: 'orange', name: 'Orange', color: '#d83b01' },
                { id: 'yellow', name: 'Yellow/Gold', color: '#ffb900' },
                { id: 'lime', name: 'Lime', color: '#bad80a' },
                { id: 'cyan', name: 'Cyan', color: '#00b7c3' },
                { id: 'navy', name: 'Navy', color: '#002050' },
                { id: 'gray', name: 'Gray', color: '#5d5a58' },
                { id: 'lightgray', name: 'Light Gray', color: '#a19f9d' },
                { id: 'brown', name: 'Brown', color: '#8e562e' },
                { id: 'pink', name: 'Pink', color: '#e3008c' }
            ]
        };
    },
    mounted() {
        // Load saved theme from user settings if logged in
        if (typeof shared !== 'undefined' && shared.isLogedIn && shared.isLogedIn()) {
            try {
                let userSettings = shared.getUserSettings();
                if (userSettings && userSettings.Theme) {
                    this.currentTheme = userSettings.Theme;
                    this.applyTheme(userSettings.Theme);
                } else {
                    // Fallback to localStorage or default
                    const savedTheme = localStorage.getItem('app-theme') || 'blue';
                    this.currentTheme = savedTheme;
                    this.applyTheme(savedTheme);
                }
            } catch (ex) {
                console.warn('Failed to load user theme:', ex);
                const savedTheme = localStorage.getItem('app-theme') || 'blue';
                this.currentTheme = savedTheme;
                this.applyTheme(savedTheme);
            }
        } else {
            // Not logged in, use localStorage
            const savedTheme = localStorage.getItem('app-theme') || 'blue';
            this.currentTheme = savedTheme;
            this.applyTheme(savedTheme);
        }
        
        // Close menu when clicking outside
        document.addEventListener('click', this.handleClickOutside);
    },
    beforeUnmount() {
        document.removeEventListener('click', this.handleClickOutside);
    },
    methods: {
        toggleMenu() {
            this.showMenu = !this.showMenu;
        },
        selectTheme(themeId) {
            this.currentTheme = themeId;
            this.applyTheme(themeId);
            
            // Save to user settings if logged in
            if (typeof shared !== 'undefined' && shared.isLogedIn && shared.isLogedIn()) {
                try {
                    let userSettings = shared.getUserSettings();
                    if (!userSettings) userSettings = {};
                    userSettings.Theme = themeId;
                    
                    // Check if setUserSettings exists
                    if (typeof shared.setUserSettings === 'function') {
                        shared.setUserSettings(userSettings);
                    } else {
                        console.warn('shared.setUserSettings is not available, saving to localStorage only');
                        localStorage.setItem('app-theme', themeId);
                    }
                } catch (ex) {
                    console.error('Failed to save theme to user settings:', ex);
                    // Fallback to localStorage
                    localStorage.setItem('app-theme', themeId);
                }
            } else {
                // Not logged in, just save to localStorage
                localStorage.setItem('app-theme', themeId);
            }
            
            this.showMenu = false;
        },
        applyTheme(themeId) {
            document.documentElement.setAttribute('data-theme', themeId);
            // Also save to localStorage as backup
            localStorage.setItem('app-theme', themeId);
        },
        handleClickOutside(event) {
            const picker = this.$el;
            if (picker && !picker.contains(event.target)) {
                this.showMenu = false;
            }
        },
        getContrastColor(hex) {
            // Simple luminance check for light/dark contrast
            if (!hex) return '#fff';
            let c = hex.substring(1); // strip #
            let r = parseInt(c.substring(0,2),16);
            let g = parseInt(c.substring(2,4),16);
            let b = parseInt(c.substring(4,6),16);
            let yiq = ((r*299)+(g*587)+(b*114))/1000;
            return (yiq >= 128) ? '#000' : '#fff';
        }
    }
};
</script>

<style scoped>
.theme-picker-container {
    display: inline-block;
    position: relative;
}

.theme-picker-button {
    min-width: 40px;
    height: 40px;
    display: flex;
    align-items: center;
    justify-content: center;
    border-radius: 8px;
    transition: all 0.2s ease;
    background-color: var(--color-bg-elevated);
    border: 1px solid rgba(var(--bs-primary-rgb), 0.08) !important;
    box-shadow: 0 1px 3px rgba(0, 0, 0, 0.08);
    padding: 0;
    font-size: 1.1rem;
    color: var(--color-text-muted);
}

.theme-picker-button:hover {
    transform: translateY(-1px);
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.12) !important;
    border-color: rgba(var(--bs-primary-rgb), 0.12) !important;
    background-color: var(--color-bg-subtle);
    color: var(--bs-primary, #0078d4);
}

.theme-picker-button:active {
    transform: translateY(0);
}

.theme-picker-menu {
    position: absolute;
    top: calc(100% + 8px);
    right: 0;
    left: auto;
    border-radius: 10px;
    border: 1px solid rgba(var(--bs-primary-rgb), 0.08);
    background: var(--color-bg-elevated);
    box-shadow: 0 4px 16px rgba(0, 0, 0, 0.1);
    z-index: 1050;
}

/* RTL Support */
[dir="rtl"] .theme-picker-menu {
    right: auto;
    left: 0;
}

.theme-grid {
    display: grid;
    grid-template-columns: repeat(3, 1fr);
    gap: 8px;
}

.theme-item {
    border: 1.5px solid transparent;
    background: transparent;
    padding: 4px;
    border-radius: 10px;
    cursor: pointer;
    transition: all 0.2s ease;
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 4px;
}

.theme-item:hover {
    border-color: rgba(var(--bs-primary-rgb), 0.06);
    background-color: var(--color-bg-subtle);
}

.theme-item.active {
    border-color: var(--bs-primary, #0078d4);
    background-color: rgba(var(--bs-primary-rgb, 0, 120, 212), 0.05);
}

.theme-color-preview {
    width: 36px;
    height: 36px;
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;
    box-shadow: 0 1px 4px rgba(0, 0, 0, 0.12), 0 1px 2px rgba(0, 0, 0, 0.08);
    transition: all 0.2s ease;
    font-size: 0.65rem;
}

.theme-item:hover .theme-color-preview {
    transform: scale(1.05);
    box-shadow: 0 2px 6px rgba(0, 0, 0, 0.16), 0 1px 3px rgba(0, 0, 0, 0.1);
}

.theme-item.active .theme-color-preview {
    box-shadow: 0 2px 5px rgba(0, 0, 0, 0.18), 0 1px 2px rgba(0, 0, 0, 0.12);
}

.theme-name {
    text-align: center;
    font-size: 0.65rem;
    color: var(--color-text-muted);
    font-weight: 500;
    white-space: nowrap;
}

.theme-item.active .theme-name {
    color: var(--bs-primary, #0078d4);
    font-weight: 600;
}
</style>
