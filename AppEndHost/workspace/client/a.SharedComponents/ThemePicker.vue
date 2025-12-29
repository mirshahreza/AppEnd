<template>
    <div class="theme-picker-container">
        <div class="dropdown">
            <button class="btn btn-sm theme-picker-button" 
                    type="button" 
                    data-bs-toggle="dropdown" 
                    aria-expanded="false"
                    :title="shared.translate('ChangeTheme')">
                <i class="fa-solid fa-palette fa-fw"></i>
            </button>
            <div class="dropdown-menu dropdown-menu-end theme-picker-menu shadow-lg p-2" style="min-width: 280px;">
                <div class="px-2 pb-2 mb-2 border-bottom">
                    <small class="text-secondary fw-bold">{{ shared.translate('SelectTheme') }}</small>
                </div>
                <div class="theme-grid">
                    <button v-for="theme in themes" 
                            :key="theme.id"
                            @click="selectTheme(theme.id)"
                            :class="['theme-item', { 'active': currentTheme === theme.id }]"
                            :title="theme.name">
                        <div class="theme-color-preview" :style="{ backgroundColor: theme.color }">
                            <i v-if="currentTheme === theme.id" class="fa-solid fa-check text-white"></i>
                        </div>
                        <div class="theme-name">
                            <small>{{ shared.translate(theme.name) }}</small>
                        </div>
                    </button>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
let _this = { 
    cid: "", 
    c: null, 
    themes: [],
    currentTheme: 'blue'
};

export default {
    setup(props) {
        _this.cid = props['cid'] || 'themePicker';
        
        // Load themes
        if (window.ThemeManager) {
            _this.themes = ThemeManager.getThemes();
            _this.currentTheme = ThemeManager.getCurrentTheme();
        }
    },
    data() { 
        return _this; 
    },
    created() { 
        _this.c = this; 
    },
    mounted() {
        // Listen for theme changes from other sources
        document.addEventListener('themeChanged', (e) => {
            _this.currentTheme = e.detail.themeId;
        });
    },
    methods: {
        selectTheme(themeId) {
            if (window.ThemeManager) {
                ThemeManager.setTheme(themeId);
                _this.currentTheme = themeId;
                
                // Show success message
                if (window.showSuccess) {
                    showSuccess(shared.translate('ThemeChanged'));
                }
            }
        }
    },
    props: { 
        cid: String 
    }
}
</script>

<style scoped>
.theme-picker-container {
    display: inline-block;
}

.theme-picker-button {
    min-width: 40px;
    height: 40px;
    display: flex;
    align-items: center;
    justify-content: center;
    border-radius: 8px;
    transition: all 0.2s ease;
    background-color: white;
    border: 1px solid rgba(0, 0, 0, 0.08) !important;
    box-shadow: 0 1px 3px rgba(0, 0, 0, 0.08);
    padding: 0;
    font-size: 1.1rem;
    color: #6c757d;
}

.theme-picker-button:hover {
    transform: translateY(-1px);
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.12) !important;
    border-color: rgba(0, 0, 0, 0.12) !important;
    background-color: #f8f9fa;
    color: var(--bs-primary, #0078d4);
}

.theme-picker-button:active {
    transform: translateY(0);
}

.theme-picker-menu {
    border-radius: 8px;
    border: 1px solid rgba(0, 0, 0, 0.08);
}

.theme-grid {
    display: grid;
    grid-template-columns: repeat(3, 1fr);
    gap: 8px;
}

.theme-item {
    border: 2px solid transparent;
    background: transparent;
    padding: 4px;
    border-radius: 8px;
    cursor: pointer;
    transition: all 0.2s ease;
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 4px;
}

.theme-item:hover {
    border-color: #dee2e6;
    background-color: #f8f9fa;
}

.theme-item.active {
    border-color: var(--bs-primary, #0078d4);
    background-color: rgba(var(--bs-primary-rgb, 0, 120, 212), 0.05);
}

.theme-color-preview {
    width: 50px;
    height: 50px;
    border-radius: 6px;
    display: flex;
    align-items: center;
    justify-content: center;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
    transition: all 0.2s ease;
}

.theme-item:hover .theme-color-preview {
    transform: scale(1.1);
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
}

.theme-name {
    text-align: center;
    font-size: 0.75rem;
    color: #6c757d;
    font-weight: 500;
}

.theme-item.active .theme-name {
    color: var(--bs-primary, #0078d4);
    font-weight: 600;
}
</style>
