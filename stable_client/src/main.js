import Vue from 'vue'
import App from './App.vue'
import { BootstrapVue, IconsPlugin } from 'bootstrap-vue';
import 'bootstrap/dist/css/bootstrap.css'
import 'bootstrap-vue/dist/bootstrap-vue.css'
import { library } from '@fortawesome/fontawesome-svg-core'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'
import { faUserShield, faHorse, faBoxes, faCalendarAlt, faUsers, faFileDownload } from '@fortawesome/free-solid-svg-icons'
import VueRouter from 'vue-router'
import Home from "@/components/Home";
import Admin from "@/components/Admin";
import Equipment from "@/components/Equipment";
import Event from "@/components/Event";
import Horse from "@/components/Horse";
import Member from "@/components/Member";

Vue.use(VueRouter)
Vue.use(BootstrapVue);
Vue.use(IconsPlugin);
library.add(faUserShield, faHorse, faBoxes, faCalendarAlt, faUsers, faFileDownload)
Vue.component('font-awesome-icon', FontAwesomeIcon)

Vue.config.productionTip = false

const routes = [
  { path: '/', redirect: '/home' },
  { path: '/home', component: Home },
  { path: '/admin', component: Admin },
  { path: '/equipment', component: Equipment },
  { path: '/event', component: Event },
  { path: '/horse', component: Horse },
  { path: '/member', component: Member },
];

const router = new VueRouter({routes})

new Vue({
  render: h => h(App),
  router
}).$mount('#app')
