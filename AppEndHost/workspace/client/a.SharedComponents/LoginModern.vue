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
            <div class="login-card" :class="{'shake': showError}">
                <!-- Logo Section with Pulse Animation -->
                <div class="logo-section">
                    <div class="logo-wrapper">
                        <img src="/a..lib/images/AppEnd-Logo-Full.png" class="logo-img rounded rounded-4" alt="AppEnd Logo" />
                        <div class="logo-glow"></div>
                    </div>
                </div>

                <!-- Welcome Text with Fade In -->
                <div class="welcome-section">
                    <h1 class="welcome-title">{{shared.translate("Welcome Back")}}</h1>
                    <p class="welcome-subtitle">{{shared.translate("Sign in to continue")}}</p>
                </div>

                <!-- Error Message with Slide Down -->
                <transition name="slide-down">
                    <div v-if="showError" class="error-message">
                        <i class="fa-solid fa-exclamation-circle"></i>
                        <span>{{shared.translate("Login failed")}}</span>
                    </div>
                </transition>

                <!-- Form Section -->
                <form @submit.prevent="submit" class="login-form">
                    <!-- Username Input with Icon -->
                    <div class="input-group-modern">
                        <i class="fa-solid fa-user input-icon"></i>
                        <input 
                            type="text" 
                            class="form-input-modern" 
                            v-model="local.UserName"
                            :placeholder="shared.translate('UserName')"
                            @key.up.enter="submit"
                            required
                            autocomplete="username"
                        />
                    </div>

                    <!-- Password Input with Icon -->
                    <div class="input-group-modern">
                        <i class="fa-solid fa-lock input-icon"></i>
                        <input 
                            :type="showPassword ? 'text' : 'password'" 
                            class="form-input-modern" 
                            v-model="local.Password"
                            :placeholder="shared.translate('Password')"
                            @key.up.enter="submit"
                            required
                            autocomplete="current-password"
                        />
                        <i 
                            class="fa-solid input-icon-right" 
                            :class="showPassword ? 'fa-eye-slash' : 'fa-eye'"
                            @click="showPassword = !showPassword"
                        ></i>
                    </div>

                    <!-- Remember Me with Modern Toggle -->
                    <div class="remember-section">
                        <label class="modern-checkbox">
                            <input type="checkbox" v-model="local.RememberMe" />
                            <span class="checkbox-mark">
                                <i class="fa-solid fa-check"></i>
                            </span>
                            <span class="checkbox-label">{{shared.translate("RememberMe")}}</span>
                        </label>
                    </div>

                    <!-- Login Button with Ripple Effect -->
                    <button type="submit" class="btn-modern-login" :class="{'loading': isLoading}" :disabled="isLoading">
                        <span class="btn-content" v-if="!isLoading">
                            <i class="fa-solid fa-sign-in-alt"></i>
                            <span>{{shared.translate("Login")}}</span>
                        </span>
                        <span class="btn-loading" v-else>
                            <i class="fa-solid fa-spinner fa-spin"></i>
                            <span>{{shared.translate("Signing in...")}}</span>
                        </span>
                        <div class="ripple"></div>
                    </button>
                </form>

                <!-- Footer -->
                <div class="login-footer">
                    <div class="footer-text">
                        A Full Stack And Low Code System
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
                    $('.form-input-modern').first().focus();
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
                
                // کاهش تعداد particles به 25 و توزیع یکنواخت
                for (let i = 0; i < 25; i++) {
                    const particle = document.createElement('div');
                    particle.className = 'particle';
                    
                    // توزیع یکنواخت در تمام صفحه
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
                $(document).on('click', '.btn-modern-login', function(e) {
                    const ripple = $(this).find('.ripple');
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

<style scoped>
/* Modern Login Container */
.modern-login-container {
    position: relative;
    width: 100%;
    height: 100vh;
    display: flex;
    align-items: center;
    justify-content: center;
    overflow: hidden;
    background: linear-gradient(135deg, #f0f4ff 0%, #e8f3ff 100%);
}

/* Animated Gradient Background */
.gradient-bg {
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background: linear-gradient(-45deg, #f7f9ff, #eef6ff, #f7fcff, #ededed);
    background-size: 400% 400%;
    animation: gradientShift 25s ease infinite;
    z-index: 0;
}

@keyframes gradientShift {
    0% { background-position: 0% 50%; }
    50% { background-position: 100% 50%; }
    100% { background-position: 0% 50%; }
}

/* Floating Shapes */
.floating-shapes {
    position: absolute;
    width: 100%;
    height: 100%;
    overflow: hidden;
    z-index: 1;
}

.shape {
    position: absolute;
    border-radius: 30%;
    backdrop-filter: blur(8px);
}

.shape-1 {
    width: 180px;
    height: 180px;
    top: 18%;
    left: 25%;
    background: rgba(121, 134, 203, 0.12);
    animation: float1 28s infinite ease-in-out;
}

.shape-2 {
    width: 140px;
    height: 140px;
    top: 22%;
    right: 28%;
    background: rgba(149, 117, 205, 0.11);
    animation: float2 32s infinite ease-in-out;
}

.shape-3 {
    width: 100px;
    height: 100px;
    top: 55%;
    left: 18%;
    background: rgba(100, 181, 246, 0.10);
    animation: float3 25s infinite ease-in-out;
}

.shape-4 {
    width: 160px;
    height: 160px;
    top: 28%;
    left: 50%;
    transform: translateX(-50%);
    background: rgba(159, 168, 218, 0.13);
    animation: float4 30s infinite ease-in-out;
}

.shape-5 {
    width: 120px;
    height: 120px;
    top: 48%;
    right: 22%;
    background: rgba(179, 157, 219, 0.11);
    animation: float5 26s infinite ease-in-out;
}

.shape-6 {
    width: 90px;
    height: 90px;
    top: 25%;
    right: 48%;
    background: rgba(92, 107, 192, 0.10);
    animation: float6 29s infinite ease-in-out;
}

.shape-7 {
    width: 110px;
    height: 110px;
    top: 58%;
    left: 58%;
    background: rgba(126, 87, 194, 0.12);
    animation: float7 27s infinite ease-in-out;
}

@keyframes float1 {
    0%, 100% { transform: translate(0, 0) rotate(0deg); }
    25% { transform: translate(15px, -12px) rotate(90deg); }
    50% { transform: translate(8px, 8px) rotate(180deg); }
    75% { transform: translate(-10px, 10px) rotate(270deg); }
}

@keyframes float2 {
    0%, 100% { transform: translate(0, 0) rotate(0deg); }
    30% { transform: translate(-12px, 15px) rotate(108deg); }
    60% { transform: translate(10px, -8px) rotate(216deg); }
    90% { transform: translate(8px, -12px) rotate(324deg); }
}

@keyframes float3 {
    0%, 100% { transform: translate(0, 0) rotate(0deg); }
    20% { transform: translate(10px, 10px) rotate(72deg); }
    40% { transform: translate(-8px, -10px) rotate(144deg); }
    60% { transform: translate(12px, -8px) rotate(216deg); }
    80% { transform: translate(-10px, 8px) rotate(288deg); }
}

@keyframes float4 {
    0%, 100% { transform: translate(0, 0) rotate(0deg); }
    33% { transform: translate(12px, -15px) rotate(120deg); }
    66% { transform: translate(-15px, 12px) rotate(240deg); }
}

@keyframes float5 {
    0%, 100% { transform: translate(0, 0) rotate(0deg); }
    25% { transform: translate(-10px, -10px) rotate(90deg); }
    50% { transform: translate(12px, 8px) rotate(180deg); }
    75% { transform: translate(8px, -12px) rotate(270deg); }
}

@keyframes float6 {
    0%, 100% { transform: translate(0, 0) rotate(0deg); }
    40% { transform: translate(8px, 12px) rotate(144deg); }
    80% { transform: translate(-12px, -8px) rotate(288deg); }
}

@keyframes float7 {
    0%, 100% { transform: translate(0, 0) rotate(0deg); }
    35% { transform: translate(-15px, 10px) rotate(126deg); }
    70% { transform: translate(10px, -15px) rotate(252deg); }
}

/* Particles */
.particles {
    position: absolute;
    width: 100%;
    height: 100%;
    z-index: 1;
    pointer-events: none;
}

.particle {
    position: absolute;
    width: 2px;
    height: 2px;
    background: rgba(99, 102, 241, 0.2);
    border-radius: 50%;
    animation: particleFloat linear infinite;
}

@keyframes particleFloat {
    0% {
        transform: translateY(0) scale(1);
        opacity: 0;
    }
    10% {
        opacity: 0.4;
    }
    90% {
        opacity: 0.4;
    }
    100% {
        transform: translateY(-100vh) scale(0);
        opacity: 0;
    }
}

/* Login Wrapper - Side by Side Layout */
.login-wrapper {
    position: relative;
    display: flex;
    align-items: stretch;
    justify-content: center;
    gap: 0;
    max-width: 1100px;
    z-index: 10;
    animation: wrapperFadeIn 0.8s ease-out;
}

@keyframes wrapperFadeIn {
    from {
        opacity: 0;
        transform: scale(0.95);
    }
    to {
        opacity: 1;
        transform: scale(1);
    }
}

/* Illustration Side */
.illustration-side {
    flex: 1;
    max-width: 550px;
    position: relative;
    z-index: 1;
    animation: slideInRight 0.8s ease-out;
}

@keyframes slideInLeft {
    from {
        opacity: 0;
        transform: translateX(-50px);
    }
    to {
        opacity: 1;
        transform: translateX(0);
    }
}

.illustration-content {
    position: relative;
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    height: 100%;
    background: linear-gradient(135deg, rgba(255, 255, 255, 0.4) 0%, rgba(255, 255, 255, 0.2) 100%);
    backdrop-filter: blur(15px);
    border-radius: 0 24px 24px 0;
    padding: 60px 50px;
    border: 1px solid rgba(255, 255, 255, 0.4);
    border-left: none;
    box-shadow: 8px 0 24px rgba(99, 102, 241, 0.08);
}

.main-illustration {
    width: 100%;
    max-width: 400px;
    height: auto;
    filter: drop-shadow(0 12px 30px rgba(255, 138, 76, 0.15));
    animation: illustrationFloat 6s ease-in-out infinite;
}

@keyframes illustrationFloat {
    0%, 100% {
        transform: translateY(0) scale(1);
    }
    50% {
        transform: translateY(-12px) scale(1.01);
    }
}

/* Login Card */
.login-card {
    position: relative;
    width: 440px;
    background: rgba(255, 255, 255, 0.85);
    backdrop-filter: blur(25px);
    border: 1px solid rgba(255, 255, 255, 0.6);
    border-right: none;
    border-radius: 24px 0 0 24px;
    padding: 50px 45px;
    box-shadow: -8px 0 24px rgba(99, 102, 241, 0.08);
    transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
    animation: slideInLeft 0.8s ease-out;
    z-index: 2;
}

@keyframes slideInRight {
    from {
        opacity: 0;
        transform: translateX(50px);
    }
    to {
        opacity: 1;
        transform: translateX(0);
    }
}

.login-card:hover {
    background: rgba(255, 255, 255, 0.95);
    backdrop-filter: blur(30px);
    box-shadow: -12px 0 32px rgba(99, 102, 241, 0.12);
}

/* Shake Animation for Error */
.shake {
    animation: shake 0.5s;
}

@keyframes shake {
    0%, 100% { transform: translateX(0); }
    25% { transform: translateX(-10px); }
    75% { transform: translateX(10px); }
}

/* Logo Section */
.logo-section {
    text-align: center;
    margin-bottom: 24px;
}

.logo-wrapper {
    position: relative;
    display: inline-block;
    animation: logoZoom 0.8s ease-out;
}

@keyframes logoZoom {
    from {
        transform: scale(0);
        opacity: 0;
    }
    to {
        transform: scale(1);
        opacity: 1;
    }
}

.logo-img {
    width: 140px;
    height: auto;
    position: relative;
    z-index: 2;
    filter: drop-shadow(0 2px 8px rgba(99, 102, 241, 0.1));
}

.logo-glow {
    position: absolute;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    width: 200px;
    height: 200px;
    background: radial-gradient(circle, rgba(99, 102, 241, 0.08) 0%, transparent 70%);
    animation: pulse 3s ease-in-out infinite;
    z-index: 1;
}

@keyframes pulse {
    0%, 100% { transform: translate(-50%, -50%) scale(1); opacity: 0.3; }
    50% { transform: translate(-50%, -50%) scale(1.1); opacity: 0.5; }
}

/* Welcome Section */
.welcome-section {
    text-align: center;
    margin-bottom: 24px;
    animation: fadeInUp 0.8s ease-out 0.2s both;
}

@keyframes fadeInUp {
    from {
        opacity: 0;
        transform: translateY(20px);
    }
    to {
        opacity: 1;
        transform: translateY(0);
    }
}

.welcome-title {
    font-size: 24px;
    font-weight: 700;
    color: #334155;
    margin: 0 0 6px 0;
    background: linear-gradient(135deg, #6366f1 0%, #a78bfa 100%);
    -webkit-background-clip: text;
    -webkit-text-fill-color: transparent;
    background-clip: text;
}

.welcome-subtitle {
    font-size: 13px;
    color: #94a3b8;
    margin: 0;
}

/* Error Message */
.error-message {
    background: linear-gradient(135deg, #fecaca 0%, #fca5a5 100%);
    color: #991b1b;
    padding: 12px 20px;
    border-radius: 6px;
    margin-bottom: 24px;
    display: flex;
    align-items: center;
    gap: 10px;
    font-size: 14px;
    font-weight: 500;
    box-shadow: 0 2px 8px rgba(239, 68, 68, 0.1);
}

.slide-down-enter-active, .slide-down-leave-active {
    transition: all 0.3s ease;
}

.slide-down-enter-from {
    opacity: 0;
    transform: translateY(-20px);
}

.slide-down-leave-to {
    opacity: 0;
    transform: translateY(-20px);
}

/* Modern Input Groups */
.input-group-modern {
    position: relative;
    margin-bottom: 20px;
    animation: fadeInUp 0.8s ease-out 0.3s both;
    padding: 2px;
    background: linear-gradient(135deg, #6366f1 0%, #a78bfa 50%, #ec4899 100%);
    background-size: 200% 200%;
    border-radius: 8px;
    opacity: 1;
    transition: all 0.6s cubic-bezier(0.4, 0, 0.2, 1);
}

.input-group-modern:nth-child(2) {
    animation-delay: 0.4s;
}

.input-group-modern::before {
    content: '';
    position: absolute;
    inset: 2px;
    background: #ffffff;
    border-radius: 6px;
    z-index: 1;
    transition: all 0.6s cubic-bezier(0.4, 0, 0.2, 1);
}

.input-group-modern:not(:focus-within) {
    background: #e2e8f0;
    animation: none;
}

.input-group-modern:focus-within {
    animation: gradientRotate 3s ease-in-out infinite;
}

@keyframes gradientRotate {
    0%, 100% {
        background-position: 0% 50%;
    }
    50% {
        background-position: 100% 50%;
    }
}

.form-input-modern {
    width: 100%;
    padding: 14px 16px 14px 48px;
    border: none;
    border-radius: 6px;
    font-size: 15px;
    color: #334155;
    background: #ffffff;
    transition: all 0.6s cubic-bezier(0.4, 0, 0.2, 1);
    outline: none;
    position: relative;
    z-index: 2;
}

.form-input-modern::placeholder {
    color: #cbd5e1;
    opacity: 1;
    transition: opacity 0.6s ease;
}

.form-input-modern:focus {
    box-shadow: 0 0 0 4px rgba(92, 107, 192, 0.12), 0 0 20px rgba(99, 102, 241, 0.3);
}

.form-input-modern:focus::placeholder {
    opacity: 0.5;
}

.input-icon {
    position: absolute;
    left: 16px;
    top: 50%;
    transform: translateY(-50%);
    color: #64748b;
    font-size: 17px;
    transition: all 0.6s cubic-bezier(0.4, 0, 0.2, 1);
    z-index: 3;
}

.input-group-modern:focus-within .input-icon {
    color: #5c6bc0;
    transform: translateY(-50%) scale(1.1);
}

.input-icon-right {
    position: absolute;
    right: 16px;
    top: 50%;
    transform: translateY(-50%);
    color: #64748b;
    font-size: 17px;
    cursor: pointer;
    transition: all 0.6s cubic-bezier(0.4, 0, 0.2, 1);
    z-index: 3;
}

.input-icon-right:hover {
    color: #5c6bc0;
    transform: translateY(-50%) scale(1.15) rotate(15deg);
}

.input-group-modern:focus-within .input-icon-right {
    color: #5c6bc0;
}

/* Modern Checkbox */
.remember-section {
    margin-bottom: 20px;
    animation: fadeInUp 0.8s ease-out 0.5s both;
}

.modern-checkbox {
    display: flex;
    align-items: center;
    gap: 12px;
    cursor: pointer;
    user-select: none;
}

.modern-checkbox input[type="checkbox"] {
    display: none;
}

.checkbox-mark {
    width: 20px;
    height: 20px;
    border: 2px solid #e2e8f0;
    border-radius: 5px;
    display: flex;
    align-items: center;
    justify-content: center;
    background: #ffffff;
    transition: all 0.3s ease;
    position: relative;
}

.checkbox-mark i {
    font-size: 12px;
    color: white;
    opacity: 0;
    transform: scale(0);
    transition: all 0.2s ease;
}

.modern-checkbox input[type="checkbox"]:checked + .checkbox-mark {
    background: linear-gradient(135deg, #5c6bc0 0%, #7e57c2 100%);
    border-color: #5c6bc0;
}

.modern-checkbox input[type="checkbox"]:checked + .checkbox-mark i {
    opacity: 1;
    transform: scale(1);
}

.checkbox-label {
    font-size: 14px;
    color: #64748b;
    font-weight: 500;
}

/* Modern Login Button */
.btn-modern-login {
    width: 100%;
    padding: 12px;
    border: none;
    border-radius: 6px;
    background: linear-gradient(135deg, #5c6bc0 0%, #7e57c2 100%);
    color: white;
    font-size: 14px;
    font-weight: 600;
    cursor: pointer;
    transition: all 0.3s ease;
    position: relative;
    overflow: hidden;
    box-shadow: 0 2px 8px rgba(92, 107, 192, 0.25);
    animation: fadeInUp 0.8s ease-out 0.6s both;
}

.btn-modern-login:hover:not(:disabled) {
    transform: translateY(-1px);
    box-shadow: 0 4px 12px rgba(92, 107, 192, 0.35);
    background: linear-gradient(135deg, #5e35b1 0%, #8e24aa 100%);
}

.btn-modern-login:active:not(:disabled) {
    transform: translateY(0);
}

.btn-modern-login:disabled {
    opacity: 0.65;
    cursor: not-allowed;
}

.btn-content,
.btn-loading {
    display: flex;
    align-items: center;
    justify-content: center;
    gap: 8px;
}

/* Ripple Effect */
.ripple {
    position: absolute;
    width: 20px;
    height: 20px;
    border-radius: 50%;
    background: rgba(255, 255, 255, 0.4);
    transform: translate(-50%, -50%) scale(0);
    pointer-events: none;
}

.ripple.active {
    animation: rippleEffect 0.6s ease-out;
}

@keyframes rippleEffect {
    to {
        transform: translate(-50%, -50%) scale(20);
        opacity: 0;
    }
}

/* Footer */
.login-footer {
    margin-top: 24px;
    text-align: center;
    animation: fadeInUp 0.8s ease-out 0.7s both;
}

.footer-badge {
    display: inline-flex;
    align-items: center;
    gap: 8px;
    padding: 8px 16px;
    background: linear-gradient(135deg, rgba(129, 140, 248, 0.08) 0%, rgba(196, 181, 253, 0.08) 100%);
    border-radius: 20px;
    color: #818cf8;
    font-size: 13px;
    font-weight: 600;
    margin-bottom: 16px;
}

.footer-text {
    font-size: 12px;
    color: #cbd5e1;
    font-weight: 500;
}

/* Responsive Design */
@media (max-width: 480px) {
    .login-wrapper {
        flex-direction: column;
        gap: 0;
        padding: 20px;
        max-width: 100%;
    }
    
    .illustration-side {
        max-width: 100%;
        order: 1;
    }
    
    .illustration-content {
        border-radius: 24px 24px 0 0;
        border: 1px solid rgba(255, 255, 255, 0.4);
        border-bottom: none;
        padding: 40px 30px;
    }
    
    .main-illustration {
        max-width: 240px;
    }
    
    .login-card {
        width: 100%;
        max-width: 100%;
        padding: 40px 30px;
        order: 2;
        border-radius: 0 0 24px 24px;
        border: 1px solid rgba(255, 255, 255, 0.6);
        border-top: none;
        z-index: 3;
    }
    
    .login-card::before {
        left: 50%;
        top: -2px;
        transform: translateX(-50%);
        width: 140px;
        height: 4px;
        border-radius: 4px;
    }
    
    .welcome-title {
        font-size: 20px;
    }
    
    .logo-img {
        width: 100px;
    }
    
    .shape {
        opacity: 0.4;
    }
    
    .shape-1 { width: 110px; height: 110px; }
    .shape-2 { width: 90px; height: 90px; }
    .shape-3 { width: 70px; height: 70px; }
    .shape-4 { width: 100px; height: 100px; }
    .shape-5 { width: 80px; height: 80px; }
    .shape-6 { width: 65px; height: 65px; }
    .shape-7 { width: 75px; height: 75px; }
}

@media (min-width: 481px) and (max-width: 768px) {
    .login-wrapper {
        flex-direction: column;
        gap: 0;
        padding: 30px;
        max-width: 550px;
    }
    
    .illustration-side {
        max-width: 100%;
        order: 1;
    }
    
    .illustration-content {
        border-radius: 24px 24px 0 0;
        border: 1px solid rgba(255, 255, 255, 0.4);
        border-bottom: none;
        padding: 50px 40px;
    }
    
    .main-illustration {
        max-width: 320px;
    }
    
    .login-card {
        width: 100%;
        max-width: 100%;
        order: 2;
        border-radius: 0 0 24px 24px;
        border: 1px solid rgba(255, 255, 255, 0.6);
        border-top: none;
        z-index: 3;
        padding: 45px 40px;
    }
    
    .login-card::before {
        left: 50%;
        top: -2px;
        transform: translateX(-50%);
        width: 160px;
        height: 4px;
        border-radius: 4px;
    }
    
    .shape-1 { width: 140px; height: 140px; }
    .shape-2 { width: 110px; height: 110px; }
    .shape-3 { width: 80px; height: 80px; }
    .shape-4 { width: 125px; height: 125px; }
    .shape-5 { width: 95px; height: 95px; }
    .shape-6 { width: 75px; height: 75px; }
    .shape-7 { width: 85px; height: 85px; }
}

@media (min-width: 769px) and (max-width: 1024px) {
    .login-wrapper {
        gap: 0;
        max-width: 900px;
    }
    
    .illustration-side {
        max-width: 450px;
    }
    
    .illustration-content {
        padding: 50px 40px;
    }
    
    .main-illustration {
        max-width: 360px;
    }
    
    .login-card {
        width: 420px;
        padding: 45px 40px;
    }
    
    .shape-1 { width: 160px; height: 160px; }
    .shape-2 { width: 125px; height: 125px; }
    .shape-3 { width: 90px; height: 90px; }
    .shape-4 { width: 145px; height: 145px; }
    .shape-5 { width: 110px; height: 110px; }
    .shape-6 { width: 85px; height: 85px; }
    .shape-7 { width: 100px; height: 100px; }
}

@media (min-width: 1025px) and (max-width: 1280px) {
    .login-wrapper {
        gap: 0;
        max-width: 1050px;
    }
    
    .illustration-side {
        max-width: 520px;
    }
    
    .illustration-content {
        padding: 55px 45px;
    }
}
</style>
