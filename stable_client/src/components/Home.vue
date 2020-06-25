<template>
    <div class="pt-5">
        <h1 class="text-center">Welcome to stable</h1>
        <div v-if="!rights.isConnected" >
            <h2 class="text-center pt-5 pb-3">Please, login</h2>
            <div class="text-center">
                <b-row class="pb-2">
                    <b-col lg="3"></b-col>
                    <b-col cols="3" lg="2" class="">
                        <label for="username" class="m-0">Username</label>
                    </b-col>
                    <b-col cols="9" lg="4" class="">
                        <b-form-input
                                id="username"
                                size="sm"
                                type="text"
                                trim
                                v-model="login.login"
                                autocomplete="off"
                        ></b-form-input>
                    </b-col>
                    <b-col lg="3"></b-col>
                </b-row>
                <b-row>
                    <b-col lg="3"></b-col>
                    <b-col cols="3" lg="2" class="">
                        <label for="password" class="m-0">Password</label>
                    </b-col>
                    <b-col cols="9" lg="4" class="">
                        <b-form-input id="password" size="sm" type="password" v-model="login.pass"></b-form-input>
                    </b-col>
                    <b-col lg="3"></b-col>
                </b-row>
                <b-row>
                    <b-col lg="4"></b-col>
                    <b-col cols="12" lg="4" class="pt-2">
                        <b-btn
                                v-on:click="$emit('login', login)"
                                :disabled="states.login.running"
                                variant="secondary"
                                class="w-100"
                        >
                            <b-spinner small variant="light" label="loading" v-if="states.login.running"></b-spinner>
                            <span v-else>Login</span>
                        </b-btn>
                    </b-col>
                    <b-col lg="4"></b-col>
                </b-row>
            </div>
        </div>
        <b-container v-else>
            <b-row>
                <b-col cols="12">
                    <h2 class="text-center pt-5 pb-3">Choose a category</h2>
                </b-col>
            </b-row>
            <b-row>
                <b-col class="p-2" cols="12" sm="6" lg="4" v-if="rights.isAdmin">
                    <b-btn to="/admin" class="w-100 huge-btn">
                        <font-awesome-icon icon="user-shield"></font-awesome-icon><br>
                        <span>Admin</span>
                    </b-btn>
                </b-col>
                <b-col class="p-2" cols="12" sm="6" lg="4">
                    <b-btn to="/horse" class="w-100 huge-btn">
                        <font-awesome-icon icon="horse"></font-awesome-icon><br>
                        <span>Horse</span>
                    </b-btn>
                </b-col>
                <b-col class="p-2" cols="12" sm="6" lg="4" v-if="rights.isAdmin || rights.isSecretary">
                    <b-btn to="/equipment" class="w-100 huge-btn">
                        <font-awesome-icon icon="boxes"></font-awesome-icon><br>
                        <span>Equipment</span>
                    </b-btn>
                </b-col>
                <b-col class="p-2" cols="12" sm="6" lg="4">
                    <b-btn to="/event" class="w-100 huge-btn">
                        <font-awesome-icon icon="calendar-alt"></font-awesome-icon><br>
                        <span>Event</span>
                    </b-btn>
                </b-col>
                <b-col class="p-2" cols="12" sm="6" lg="4">
                    <b-btn to="/member" class="w-100 huge-btn">
                        <font-awesome-icon icon="users"></font-awesome-icon><br>
                        <span>Member</span>
                    </b-btn>
                </b-col>
            </b-row>



        </b-container>
    </div>
</template>

<script>
    export default {
        name: "Home",
        props: {
            rights : Object,
            states : Object,
        },
        data: function () {
            return {
                login : {
                    login : '',
                    pass : ''
                }
            }
        }
    }
</script>

<style lang="scss" scoped>
    .rounded-top-left {
        border-top-left-radius: .25rem !important;
    }
    .rounded-top-right {
        border-top-right-radius: .25rem !important;
    }
    .rounded-bottom-right {
        border-bottom-right-radius: .25rem !important;
    }
    .rounded-bottom-left {
        border-bottom-left-radius: .25rem !important;
    }
    .huge-btn {
        width: 100% !important;
        height: 15rem !important;
        font-size: 4rem;
        span {
            font-size: 2rem;
        }
    }
    .form-control,
    input:-webkit-autofill,
    input:-webkit-autofill:hover,
    input:-webkit-autofill:focus,
    input:-webkit-autofill:active
    {
        -webkit-text-fill-color:rgba(255, 255, 255, 0.8) !important;
        color: rgba(255, 255, 255, 0.8) !important;
        background-color: rgba(255, 255, 255, 0.2) !important;
    }
</style>