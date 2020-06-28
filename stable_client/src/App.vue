<template>
  <div id="app">
    <b-navbar class="container" toggleable="lg" type="dark" variant="dark">
      <b-navbar-toggle target="nav-collapse"></b-navbar-toggle>
      <b-collapse id="nav-collapse" is-nav>
        <b-navbar-nav>
          <b-nav-item to="/home">Home</b-nav-item>
          <b-nav-item v-if="rights.isConnected" to="/event">Event</b-nav-item>
          <b-nav-item v-if="rights.isConnected" to="/member">Member</b-nav-item>
          <b-nav-item v-if="rights.isConnected" to="/horse">Horse</b-nav-item>
          <b-nav-item v-if="rights.isAdmin || rights.isSecretary" to="/equipment">Equipment</b-nav-item>
          <b-nav-item v-if="rights.isAdmin" to="/admin" disabled>Admin</b-nav-item>
        </b-navbar-nav>
      </b-collapse>
      <b-navbar-nav class="ml-auto">
        <b-nav-item v-if="!rights.isConnected" disabled><em>Not logged in</em></b-nav-item>
        <b-nav-item v-on:click="logout" v-else>Logout</b-nav-item>
      </b-navbar-nav>
    </b-navbar>
    <router-view
            class="container"
            v-bind:base-url="baseUrl"
            v-bind:rights="rights"
            v-bind:states="states"
            v-bind:owners="owners"
            v-bind:boxes="boxes"
            v-on:error="handleError"
            v-on:refresh-owner="getOwners"
            v-on:refresh-box="getBoxes"
            v-on:login="login">
    </router-view>
    <b-toast id="error_toast" variant="danger" solid toaster="b-toaster-top-center">
      <template v-slot:toast-title>
        <div class="d-flex flex-grow-1 align-items-baseline">
          <b-img blank blank-color="#ff5555" class="mr-2 rounded" width="12" height="12" ></b-img>
          <strong class="mr-auto toast-text">{{ toast.title }}</strong>
        </div>
      </template>
      <div class="toast-text">{{ toast.body }}</div>
    </b-toast>
  </div>
</template>

<script>
  import axios from 'axios';

  export default {
    name: 'App',
    components: {
    },
    mounted: function() {
      this.getRights();
    },
    data : function() {
      return {
        rights : {
          isConnected : false,
          isSecretary : false,
          isGroom : false,
          isAdmin : false
        },
        baseUrl : 'https://localhost:5001',
        states : {
          login : {
            running : false,
            error : false
          }
        },
        toast : {
          title : '',
          body  : ''
        },
        owners : [
        ],
        boxes : [
        ]
      }
    },
    methods : {
      login : function (loginData) {
        let that = this;
        that.states.login.running = true
        axios.post(this.baseUrl + '/auth/login', loginData, { withCredentials: true })
                .then(() => {
                  that.getRights();
                  that.states.login.running = false;
                  that.states.login.error = false;
                })
                .catch(error => {
                  that.handleError({ title: "Error during login", error: error });
                  that.states.login.running = false;
                  that.states.login.error = true;
                })
      },
      getRights : function () {
        let that = this;
        axios.get(this.baseUrl + '/auth/rights', { withCredentials: true })
                .then(function (response) {
                  that.rights = response.data;
                }).catch(error => {
          that.handleError({ title: "Error while retrieving user rights", error: error })
          that.states.login.running = false;
          that.states.login.error = true;
          that.rights.isConnected = false;
        })
      },
      logout: function() {
        let that = this;
        axios.post(this.baseUrl + '/auth/logout', null, { withCredentials: true })
                .then (() => {
                  that.rights = {
                    isConnected : false,
                    isSecretary : false,
                    isGroom : false,
                    isAdmin : false
                  };
                  that.$router.push('/home');
                }).catch(error => {
          that.handleError({ title: "Error while logging out", error: error })
          that.rights.isConnected = false;
          that.getRights();
        })
      },
      getOwners: function() {
        let that = this;
        axios.get(this.baseUrl + '/person', { withCredentials: true })
                .then(function (response) {
                  that.owners = response.data;
                  that.owners.forEach((owner) => { owner.fullname = owner.name + " " + owner.surname})
                }).catch(error => {
          that.handleError({ title: "Error while retrieving member list", error: error })
        });
      },
      getBoxes: function() {
        let that = this;
        axios.get(this.baseUrl + '/box', { withCredentials: true })
                .then(function (response) {
                  that.boxes = response.data;
                }).catch(error => {
          that.handleError({ title: "Error while retrieving box list", error: error })
        })
      },
      handleError : function (errorData) {
        console.error(errorData.error);
        this.toast.title = errorData.title;
        if (errorData.error.response !== undefined && errorData.error.response.status !== undefined) {
          this.toast.body = "(http " + errorData.error.response.status + ") ";
        } else {
          this.toast.body = "(Status unknown) ";
        }
        if (errorData.error.response !== undefined && errorData.error.response.data !== undefined) {
          if (errorData.error.response.data.title !== undefined) {
            this.toast.body += errorData.error.response.data.title;
          } else {
            this.toast.body += errorData.error.response.data;
          }

        } else if (errorData.error.message !== undefined) {
          this.toast.body += errorData.error.message;
        } else {
          this.toast += 'Unspecified error';
        }
        this.$bvToast.show('error_toast');
      }
    }
  }
</script>

<style lang="scss">
  @import url('https://fonts.googleapis.com/css?family=Roboto');
  * {
    color: rgba(255, 255, 255, 0.8);
  }
  .text-dark {
    color: rgba(255, 255, 255, 0.8) !important;
  }
  .toast-text {
    color: rgba(0, 0, 0, 0.8);
  }
  body {
    background-color: #343a40 !important;
    font-family: 'roboto', sans-serif;
  }
  .nav-link {
    &.router-link-active {
      border-bottom: 1px solid;
    }
  }
  .form-control, .custom-select {
    bdi, .b-calendar-grid-caption {
      color: #495057 !important;
    }
    .b-calendar-grid-weekdays {
      small {
        color: #495057 !important;
      }
    }
    &:not(.col.p-0) {
      background-color: rgba(52, 58, 64, 0.8) !important;
      border-color: #454d55 !important;
      color: rgba(255, 255, 255, 0.8) !important;
    }
    label {
      color: rgba(255, 255, 255, 0.8) !important;
    }
  }
  .bi-chevron-up, .bi-chevron-down, .b-form-time-control div div .bi-circle-fill {
    color: rgba(52, 58, 64, 0.8) !important;
    fill: rgba(52, 58, 64, 0.8) !important;
  }
  .btn {
    &.btn-sm {
      &.h-auto {
        font-size: 1px !important;
        max-width: 1px;
        width: 1px;
        padding-left: 1px;
        svg {
          display: none;
        }
      }
    }

  }
</style>
