import './css/site.scss';

// import "babel-polyfill";
import Vue from 'vue';

// Vuetify imports
import Vuetify from 'vuetify'
import 'vuetify/dist/vuetify.min.css'
Vue.use(Vuetify);

import App from './components/app.vue';
import { AxiosClient } from 'coalesce-vue'

import VueRouter from 'vue-router';
Vue.use(VueRouter);

import $metadata from '@/metadata.g';

// viewmodels.g has sideeffects - it populates the global lookup on ViewModel and ListViewModel.
import '@/viewmodels.g';

import CoalesceVuetify, { 
    CAdminTablePage 
} from '../../coalesce-vue-vuetify/src';
Vue.use(CoalesceVuetify, {
  metadata: $metadata
});

// @ts-ignore
const components: any = Vue.options.components;
components.VInput.options.props.dense.default = true
components.VTextField.options.props.dense.default = true

Vue.config.productionTip = false;

AxiosClient.defaults.baseURL = '/api'
AxiosClient.defaults.withCredentials = true

const router = new VueRouter({ mode: 'history', routes: [
    { path: '/', name: 'home', component: CAdminTablePage, props: { modelName: 'Person', class:  "ma-4" } },
]});

new Vue({
    el: '#app',
    router,
    vuetify: new Vuetify({
      icons: {
        //iconfont: 'fa', // 'mdi' || 'mdiSvg' || 'md' || 'fa' || 'fa4'
      },
      customProperties: true,
      theme: { 
        options: {
          customProperties: true,
        },
        themes: {
          light: {
            // primary: "#9ccc6f",
            // secondary: "#4d97bc",
            // accent: "#e98f07",
            // error: '#ff0000',
          }
        }
      }
    }),
    render: h => h(App)
});
