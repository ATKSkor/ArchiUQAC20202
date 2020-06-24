<template>
    <b-container class="pt-2">
        <h2 class="text-center">Horse Management</h2>
        <b-table-simple hover small responsive>
            <b-thead head-variant="dark">
                <b-tr>
                    <b-th class="text-center">Name</b-th>
                    <b-th class="text-center">Box #</b-th>
                    <b-th class="text-center">Owner</b-th>
                    <b-th class="text-center">Medic Entries</b-th>
                </b-tr>
            </b-thead>
            <b-tbody >
                <b-tr
                        v-for="horse in horses"
                        :key="horse.id"
                >
                    <b-td class="text-center">{{ horse.name }}</b-td>
                    <b-td class="text-center">{{ horse.boxID }}</b-td>
                    <b-td class="text-center">{{ horse.ownerFullName }}</b-td>
                    <b-td class="text-center">
                        {{ horse.medicEntryIDs.length === 0 ? 'none' : horse.medicEntryIDs.length }}
                        <font-awesome-icon
                                class="mx-2"
                                icon="file-download"
                                v-if="horse.medicEntryIDs.length > 0">

                        </font-awesome-icon>
                    </b-td>
                </b-tr>
            </b-tbody>
        </b-table-simple>
        <h4 class="text-center" v-if="horses === undefined || horses.length === 0">No horse found</h4>
    </b-container>
</template>

<script>
    import axios from "axios";

    export default {
        name: "Horse.vue",
        mounted: function() {
            this.getAll();
        },
        props: {
            baseUrl: String
        },
        data: function () {
            return {
                horses : []
            }
        },
        methods: {
            getAll : function () {
                let that = this;
                axios.get(this.baseUrl + '/horse', { withCredentials: true })
                    .then(function (response) {
                        that.horses = response.data;
                    }).catch(error => {
                    that.$emit('error', { title : "Error while retrieving horses list", error : error })
                })
            }
        }
    }
</script>

<style lang="scss" scoped>
    tbody {
        .fa-file-download:hover {
            cursor: pointer;
            * {
                color: #007bff;
            }
        }
    }

</style>