<template>
    <div class="modern-login-container">
        <!-- Background Animated Gradient -->
        <div class="gradient-bg"></div>
        
        <!-- Floating Shapes Animation -->
        <div class="floating-shapes">
            <div class="shape shape-1"></div>
            <div class="shape shape-2"></div>
            <div class="shape shape-3"></div>
            <div class="shape shape-4"></div>
            <div class="shape shape-5"></div>
            <div class="shape shape-6"></div>
            <div class="shape shape-7"></div>
        </div>

        <!-- Main Login Wrapper -->
        <div class="login-wrapper">
            <!-- Left Side - Login Card -->
            <div class="login-card ae-glass-card" :class="{'ae-shake': showError}">
                <!-- Logo Section with Pulse Animation -->
                <div class="logo-section">
                    <div class="logo-wrapper">
                        <img src="/a..lib/images/AppEnd-Logo-Full.png" class="logo-img rounded rounded-4" alt="AppEnd Logo" />
                        <div class="ae-logo-glow"></div>
                    </div>
                </div>

                <!-- Welcome Text with Fade In -->
                <div class="welcome-section ae-fade-in-up">
                    <h1 class="welcome-title ae-text-gradient-primary">{{shared.translate("Welcome Back")}}</h1>
                </div>

                <!-- Error Message with Slide Down -->
                <transition name="ae-slide-down">
                    <div v-if="showError" class="ae-alert ae-alert-error">
                        <i class="fa-solid fa-exclamation-circle"></i>
                        <span>{{shared.translate("Login failed")}}</span>
                    </div>
                </transition>

                <!-- Form Section -->
                <form @submit.prevent="submit" class="login-form">
                    <!-- Username Input with Icon -->
                    <div class="ae-input-group">
                        <i class="fa-solid fa-user ae-input-icon"></i>
                        <input 
                            type="text" 
                            class="ae-input" 
                            v-model="local.UserName"
                            :placeholder="shared.translate('UserName')"
                            @key.up.enter="submit"
                            required
                            autocomplete="username"
                        />
                    </div>

                    <!-- Password Input with Icon -->
                    <div class="ae-input-group">
                        <i class="fa-solid fa-lock ae-input-icon"></i>
                        <input 
                            :type="showPassword ? 'text' : 'password'" 
                            class="ae-input" 
                            v-model="local.Password"
                            :placeholder="shared.translate('Password')"
                            @key.up.enter="submit"
                            required
                            autocomplete="current-password"
                        />
                        <i 
                            class="fa-solid ae-input-icon-right" 
                            :class="showPassword ? 'fa-eye-slash' : 'fa-eye'"
                            @click="showPassword = !showPassword"
                        ></i>
                    </div>

                    <!-- Remember Me with Modern Toggle -->
                    <div class="remember-section">
                        <label class="ae-checkbox">
                            <input type="checkbox" v-model="local.RememberMe" />
                            <span class="ae-checkbox-mark">
                                <i class="fa-solid fa-check"></i>
                            </span>
                            <span class="ae-checkbox-label">{{shared.translate("RememberMe")}}</span>
                        </label>
                    </div>

                    <!-- Login Button with Ripple Effect -->
                    <button type="submit" class="ae-btn-primary" :class="{'loading': isLoading}" :disabled="isLoading">
                        <span class="ae-btn-content" v-if="!isLoading">
                            <i class="fa-solid fa-sign-in-alt"></i>
                            <span>{{shared.translate("Login")}}</span>
                        </span>
                        <span class="ae-btn-content" v-else>
                            <i class="fa-solid fa-spinner fa-spin"></i>
                            <span>{{shared.translate("Signing in...")}}</span>
                        </span>
                        <div class="ae-btn-ripple"></div>
                    </button>
                </form>

                <!-- Footer -->
                <div class="login-footer">
                    <div class="footer-text fs-d8">
                        An AI-powered studio for full-stack, low-code development
                    </div>
                </div>
            </div>

            <!-- Right Side - Illustration -->
            <div class="illustration-side">
                <div class="illustration-content">
                    <img src="/a..lib/images/i1.jpg" class="main-illustration" alt="Welcome Illustration" />
                </div>
            </div>
        </div>

        <!-- Particles Effect -->
        <div class="particles" id="particles-container"></div>
    </div>
</template>

<script>
    let _this = { 
        cid: "", 
        c: null, 
        inputs: {}, 
        local: {},
        showError: false,
        isLoading: false,
        showPassword: false
    };
    
    _this.local = { UserName: "", Password: "", RememberMe: false };
    
    export default {
        setup(props) { 
            _this.cid = props['cid']; 
        },
        data() { 
            return _this; 
        },
        created() { 
            _this.c = this; 
        },
        mounted() {
            $(document).ready(() => {
                setTimeout(() => {
                    $('.ae-input').first().focus();
                }, 500);
            });
            
            // Create particles
            this.createParticles();
            
            // Add ripple effect to button
            this.addRippleEffect();
        },
        methods: {
            submit() {
                if (_this.isLoading) return;
                
                _this.isLoading = true;
                _this.showError = false;

                // Simulate loading for better UX
                setTimeout(() => {
                    let r = shared.login(_this.local);
                    if (r !== true) {
                        _this.isLoading = false;
                        _this.showError = true;
                        
                        // Hide error after 3 seconds
                        setTimeout(() => {
                            _this.showError = false;
                        }, 3000);
                    } else {
                        // Success animation
                        setTimeout(() => {
                            refereshPage();
                        }, 500);
                    }
                }, 800);
            },
            createParticles() {
                const container = document.getElementById('particles-container');
                if (!container) return;
                
                for (let i = 0; i < 25; i++) {
                    const particle = document.createElement('div');
                    particle.className = 'particle';
                    
                    const col = i % 5;
                    const row = Math.floor(i / 5);
                    
                    particle.style.left = (col * 20 + Math.random() * 15) + '%';
                    particle.style.top = (row * 20 + Math.random() * 15) + '%';
                    particle.style.animationDelay = Math.random() * 4 + 's';
                    particle.style.animationDuration = (Math.random() * 4 + 3) + 's';
                    
                    container.appendChild(particle);
                }
            },
            addRippleEffect() {
                $(document).on('click', '.ae-btn-primary', function(e) {
                    const ripple = $(this).find('.ae-btn-ripple');
                    const rect = this.getBoundingClientRect();
                    const x = e.clientX - rect.left;
                    const y = e.clientY - rect.top;
                    
                    ripple.css({
                        left: x + 'px',
                        top: y + 'px'
                    }).addClass('active');
                    
                    setTimeout(() => {
                        ripple.removeClass('active');
                    }, 600);
                });
            }
        }
    }
</script>

<style>
/* All styles moved to /a..lib/append-client.css */
</style>


