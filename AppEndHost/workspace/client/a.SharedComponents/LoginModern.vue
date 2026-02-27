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
                        <img src="/a..lib/images/AppEnd-Logo-Full.png" class="logo-img rounded rounded-4 border" alt="AppEnd Logo" style="width:175px;" />
                    </div>
                </div>
                <div class="ae-logo-glow"></div>

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
                        <input type="text"
                               class="ae-input"
                               v-model="local.UserName"
                               :placeholder="shared.translate('UserName')"
                               @keyup.enter="submit"
                               required
                               autocomplete="username" />
                    </div>

                    <!-- Password Input with Icon -->
                    <div class="ae-input-group">
                        <i class="fa-solid fa-lock ae-input-icon"></i>
                        <input :type="showPassword ? 'text' : 'password'"
                               class="ae-input"
                               v-model="local.Password"
                               :placeholder="shared.translate('Password')"
                               @keyup.enter="submit"
                               required
                               autocomplete="current-password" />
                        <i class="fa-solid ae-input-icon-right"
                           :class="showPassword ? 'fa-eye-slash' : 'fa-eye'"
                           @click="showPassword = !showPassword"></i>
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
                    <button type="submit" class="ae-btn-primary ae-btn-elegant" :class="{'loading': isLoading}" :disabled="isLoading">
                        <span class="ae-btn-content" v-if="!isLoading">
                            <i class="fa-solid fa-right-to-bracket"></i>
                            <span>{{shared.translate("Login")}}</span>
                        </span>
                        <span class="ae-btn-content" v-else>
                            <i class="fa-solid fa-spinner fa-spin"></i>
                            <span>{{shared.translate("Signing in...")}}</span>
                        </span>
                        <div class="ae-btn-ripple"></div>
                    </button>
                </form>
                <div class="login-footer"></div>
            </div>

            <!-- Right Side - Illustration Carousel -->
            <div class="illustration-side">
                <div class="illustration-content">
                    <transition :name="slideTransition" mode="out-in">
                        <div :key="currentSlide" class="illustration-slide">
                            <img :src="slides[currentSlide].image" class="main-illustration" :alt="slides[currentSlide].caption" />
                            <div class="illustration-caption-below">
                                <p class="caption-text-below">{{ slides[currentSlide].caption }}</p>
                            </div>
                        </div>
                    </transition>
                </div>
            </div>
        </div>

        <!-- Footer text below both boxes -->
        <div class="login-global-footer text-secondary fs-d9">
            <div class="text-center mb-1 text-secondary"><span class="fw-bold text-primary">Operating System</span> for applications :|</div>
            <div class="text-center mb-4 text-secondary fs-d8">An AI-powered studio for full-stack and low-code development</div>
        </div>

        <!-- Particles Effect -->
        <div class="particles" id="particles-container"></div>
    </div>
</template>

<script>
    export default {
        data() { 
            return {
                cid: "", 
                showError: false,
                isLoading: false,
                showPassword: false,
                currentSlide: 0,
                slideTransition: 'bounce-scale',
                carouselInterval: null,
                local: { UserName: "", Password: "", RememberMe: false },
                slides: [
                    {
                        image: '/a..lib/images/i1.jpg',
                        caption: 'Build powerful applications with AI-driven automation'
                    },
                    {
                        image: '/a..lib/images/i2.jpg',
                        caption: 'Transform ideas into reality with low-code innovation'
                    },
                    {
                        image: '/a..lib/images/i3.jpg',
                        caption: 'Accelerate development with intelligent code generation'
                    },
                    {
                        image: '/a..lib/images/i4.jpg',
                        caption: 'Seamless full-stack development in one platform'
                    },
                    {
                        image: '/a..lib/images/i5.jpg',
                        caption: 'Empower your team with next-gen development tools'
                    }
                ]
            };
        },
        mounted() {
            setTimeout(() => {
                const firstInput = this.$el.querySelector('.ae-input');
                if (firstInput) firstInput.focus();
            }, 500);
            
            this.createParticles();
            this.addRippleEffect();
            this.startCarousel();
        },
        beforeUnmount() {
            if (this.carouselInterval) {
                clearInterval(this.carouselInterval);
                this.carouselInterval = null;
            }
        },
        methods: {
            submit() {
                if (this.isLoading) return;
                
                this.isLoading = true;
                this.showError = false;

                setTimeout(() => {
                    let r = shared.login(this.local);
                    if (r !== true) {
                        this.isLoading = false;
                        this.showError = true;
                        
                        setTimeout(() => {
                            this.showError = false;
                        }, 3000);
                    } else {
                        setTimeout(() => {
                            refereshPage();
                        }, 500);
                    }
                }, 800);
            },
            startCarousel() {
                if (this.carouselInterval) {
                    clearInterval(this.carouselInterval);
                }
                this.carouselInterval = setInterval(() => {
                    this.slideTransition = 'fade';
                    this.currentSlide = (this.currentSlide + 1) % this.slides.length;
                }, 6000);
            },
            nextSlide() {
                this.slideTransition = 'fade';
                this.currentSlide = (this.currentSlide + 1) % this.slides.length;
            },
            goToSlide(index) {
                if (index !== this.currentSlide) {
                    this.slideTransition = 'slide-fade';
                    this.currentSlide = index;
                    
                    if (this.carouselInterval) {
                        clearInterval(this.carouselInterval);
                        this.startCarousel();
                    }
                }
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
                const self = this;
                setTimeout(() => {
                    const btn = self.$el.querySelector('.ae-btn-primary');
                    if (btn) {
                        btn.addEventListener('click', function(e) {
                            const ripple = this.querySelector('.ae-btn-ripple');
                            if (ripple) {
                                const rect = this.getBoundingClientRect();
                                const x = e.clientX - rect.left;
                                const y = e.clientY - rect.top;
                                
                                ripple.style.left = x + 'px';
                                ripple.style.top = y + 'px';
                                ripple.classList.add('active');
                                
                                setTimeout(() => {
                                    ripple.classList.remove('active');
                                }, 600);
                            }
                        });
                    }
                }, 100);
            }
        }
    }
</script>

<style scoped>
/* All login styles have been moved to a..lib/append-login.css */
</style>
